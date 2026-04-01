using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardTagView : UIView
{
    private const string BonusTrigger = "Bonus";

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Transform tagUIHolder;

    [SerializeField]
    private GameObject tagUIPrefab;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private float delayBetweenScoreCount = 0.5f;

    [SerializeField]
    private GameObject craftButton;

    [SerializeField]
    private GameObject validateButton;

    [SerializeField]
    private CraftedRecipeDayUI craftedRecipeDayUI;

    [SerializeField]
    private BonusTagGameUI bonusTagGameUI;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI basePriceText;

    public CraftingTable CurrentCraftingTable { get; set; }

    private List<Item> items = new List<Item>();
    private List<TagValue> tags = new List<TagValue>();
    private List<TagIconUI> tagsUI = new List<TagIconUI>();

    private CraftedObjectRecipe craftedObjectRecipe;
    private CraftedObjectData craftedObjectData;
    private bool isNew = false;

    private int score;
    private int rarityBoost;
    private StatModifier modifier;

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            craftButton.SetActive(true);
            validateButton.SetActive(false);
        }
        else
        {
            for (int i = tagsUI.Count - 1; i >= 0; i--)
            {
                TagIconUI tagUI = tagsUI[i];
                tagUI.OnDragReleased -= UpdateUIOrder;
                tagUI.OnDragStarted -= OnDragTagStarted;
                Destroy(tagUI.Anchor);
                Destroy(tagUI.gameObject);
            }

            items.Clear();
            tags.Clear();
            tagsUI.Clear();

            scoreText.text = "";

            bonusTagGameUI.gameObject.SetActive(false);
        }
    }

    public void Setup(List<Item> itemConsumed)
    {
        items.AddRange(itemConsumed);
        craftedObjectRecipe = managerRefs.CraftingManager.PoolCraftedItem(items, out isNew, out tags);
        craftedObjectData = managerRefs.CraftingManager.GetCraftedData(craftedObjectRecipe, isNew);
        craftedRecipeDayUI.Setup(craftedObjectRecipe, isNew);

        basePriceText.text = craftedObjectData.GetPrice().ToString();

        foreach (TagValue tagValue in tags)
        {
            TagIconUI tagUI = Instantiate(tagUIPrefab, tagUIHolder).GetComponent<TagIconUI>();
            tagUI.Setup(tagValue);
            tagsUI.Add(tagUI);
            tagUI.OnDragReleased += UpdateUIOrder;
            tagUI.OnDragStarted += OnDragTagStarted;
        }

        EventSystem.current.SetSelectedGameObject(tagsUI[0].gameObject);
        ApplyPreSelectionTags();
    }

    private void RemovePreSelectionTags()
    {
        for (int i = 0; i < tagsUI.Count; i++)
        {
            TagIconUI tagUI = tagsUI[i];
            tagUI.RemovePreSelectionTag(tags, i);
        }
    }

    private void ApplyPreSelectionTags()
    {
        for (int i = 0; i < tagsUI.Count; i++)
        {
            TagIconUI tagUI = tagsUI[i];
            tagUI.ApplyPreSelectionTag(tags, i);
        }
    }

    //UI Advanced button setup
    public void OnCraftClick()
    {
        foreach (TagIconUI tagUI in tagsUI)
        {
            tagUI.BlockOrdering();
        }

        craftButton.SetActive(false);
        StartCoroutine(CountScores());
    }

    private IEnumerator CountScores()
    {
        score = 0;
        rarityBoost = 0;
        modifier = null;

        foreach (TagIconUI tagUI in tagsUI)
        {
            score = tagUI.CountTag(score);
            scoreText.text = score.ToString();
            yield return new WaitForSecondsRealtime(delayBetweenScoreCount);
        }

        ScoreBonus();
        DisplayBonus();

        validateButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(validateButton);
    }

    //UI Advanced button setup
    public void OnValidateClick()
    {
        CraftedObject craftedObject = managerRefs.CraftingManager.CraftItem(craftedObjectData, rarityBoost, modifier);
        CurrentCraftingTable.SpawnCraftedItem(craftedObject);
        managerRefs.GameEventsManager.craftEvents.CraftItem(craftedObject.CraftedData);
        managerRefs.UIManager.ToggleCardTagView(false, CurrentCraftingTable);
    }

    private void ScoreBonus()
    {
        if (score > craftedObjectRecipe.TargetScore)
        {
            rarityBoost++;
        }
        else
        {
            modifier = craftedObjectRecipe.Rarity.MaxStatModifier.Clone(craftedObjectRecipe.Rarity.MaxStatModifier);
            modifier.Value *= 1 + (modifier.Value - 1) * (score / craftedObjectRecipe.TargetScore);
        }
    }

    private void DisplayBonus()
    {
        ModifiableValue priceModif = new ModifiableValue();
        int basePrice = craftedObjectData.GetPrice();
        priceModif.BaseValue = basePrice;
        priceModif.AddModifier(modifier);

        bonusTagGameUI.gameObject.SetActive(true);
        animator.SetTrigger(BonusTrigger);
        if (rarityBoost > 0)
        {
            bonusTagGameUI.Setup(rarityBoost, true);
        }
        else
        {
            bonusTagGameUI.Setup((int)(priceModif.Value - priceModif.BaseValue), false);
        }
    }

    private void UpdateUIOrder()
    {
        tagsUI.Sort();

        tags.Clear();
        foreach (TagIconUI tagUI in tagsUI)
        {
            tags.Add(tagUI.TagValue);
        }

        ApplyPreSelectionTags();
    }

    private void OnDragTagStarted()
    {
        RemovePreSelectionTags();
    }
}
