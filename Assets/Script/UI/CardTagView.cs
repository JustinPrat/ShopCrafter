using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardTagView : UIView
{
    private const string BonusTrigger = "Bonus";

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
    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI basePriceText;

    [SerializeField]
    private Slider sliderPoints;

    [SerializeField]
    private TextMeshProUGUI maxPointBonus;

    [SerializeField]
    private TextMeshProUGUI currentPointBonus;

    [SerializeField]
    private TextMeshProUGUI currentPointValue;

    [SerializeField]
    private Transform currentValuesTransform;

    [SerializeField]
    private float sliderDuration = 2f;

    [SerializeField]
    private AnimationCurve sliderAnimCurve;

    public CraftingTable CurrentCraftingTable { get; set; }

    private List<Item> items = new List<Item>();
    private List<TagValue> tags = new List<TagValue>();
    private List<TagIconUI> tagsUI = new List<TagIconUI>();

    private CraftedObjectRecipe craftedObjectRecipe;
    private CraftedObjectData craftedObjectData;
    private bool isNew = false;

    private int score;
    //private int rarityBoost;
    private StatModifier modifier;

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            craftButton.SetActive(true);
            validateButton.SetActive(false);
            currentValuesTransform.gameObject.SetActive(false);
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
            CurrentCraftingTable.ExitInteract();
        }
    }

    public void Setup(List<Item> itemConsumed)
    {
        items.AddRange(itemConsumed);
        craftedObjectRecipe = managerRefs.CraftingManager.PreviewPoolCraftedItem(items, out isNew, out tags);
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

        if (managerRefs.InputManager.IsGamepad)
        {
            EventSystem.current.SetSelectedGameObject(tagsUI[0].gameObject);
        }

        maxPointBonus.text = craftedObjectRecipe.TargetScore.ToString();
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
    public void OnValidateClick()
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
        //rarityBoost = 0;
        modifier = null;

        foreach (TagIconUI tagUI in tagsUI)
        {
            score = tagUI.CountTag(score);
            scoreText.text = score.ToString();
            yield return new WaitForSecondsRealtime(delayBetweenScoreCount);
        }

        ScoreBonus();
        StartCoroutine(OnScoreBonus(Mathf.Min((float)score / craftedObjectRecipe.TargetScore, 1)));
    }

    //UI Advanced button setup
    public void OnCraftClick()
    {
        RemovePreSelectionTags();
        managerRefs.UIManager.ToggleCardTagView(false, CurrentCraftingTable);

        craftedObjectData.AddPriceModifier(modifier);
        managerRefs.UIManager.ShowMiniGameView(CurrentCraftingTable, craftedObjectData, CurrentCraftingTable.GetValidUIPos());
    }

    private void ScoreBonus()
    {
        modifier = craftedObjectRecipe.Rarity.MaxStatModifier.Clone(craftedObjectRecipe.Rarity.MaxStatModifier);
        modifier.Value *= 1 + (modifier.Value - 1) * Mathf.Min((float)score / craftedObjectRecipe.TargetScore, 1);
    }

    private IEnumerator OnScoreBonus(float target)
    {
        float startValue = 0f;
        float elapsedTime = 0f;

        currentValuesTransform.gameObject.SetActive(true);

        while (elapsedTime < sliderDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / sliderDuration);

            float curveProgress = sliderAnimCurve.Evaluate(t);
            float current = Mathf.LerpUnclamped(startValue, target, curveProgress);

            sliderPoints.value = current;
            DisplayBonus(current);

            yield return null;
        }

        sliderPoints.value = target;
        DisplayBonus(target);
        validateButton.SetActive(true);

        if (managerRefs.InputManager.IsGamepad)
        {
            EventSystem.current.SetSelectedGameObject(validateButton);
        }
    }

    private void DisplayBonus(float current)
    {
        StatModifier tmpModif = craftedObjectRecipe.Rarity.MaxStatModifier.Clone(craftedObjectRecipe.Rarity.MaxStatModifier);
        tmpModif.Value *= 1 + (tmpModif.Value - 1) * Mathf.Min(current, 1);

        ModifiableValue priceModif = new ModifiableValue();
        int basePrice = craftedObjectData.GetPrice();
        priceModif.BaseValue = basePrice;
        priceModif.AddModifier(tmpModif);

        currentPointBonus.text = "+" + (priceModif.Value - priceModif.BaseValue).ToString();
        currentPointValue.text = (craftedObjectRecipe.TargetScore * current).ToString("F0");
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
