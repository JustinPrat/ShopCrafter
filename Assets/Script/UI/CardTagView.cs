using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardTagView : UIView
{
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


    public CraftingTable CurrentCraftingTable { get; set; }

    private List<Item> items = new List<Item>();
    private List<TagValue> tags = new List<TagValue>();
    private List<TagIconUI> tagsUI = new List<TagIconUI>();

    private CraftedObjectRecipe craftedObjectRecipe;
    private bool isNew = false;

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
                Destroy(tagUI.Anchor);
                Destroy(tagUI.gameObject);
            }

            items.Clear();
            tags.Clear();
            tagsUI.Clear();

            scoreText.text = "";
        }
    }

    public void Setup(List<Item> itemConsumed)
    {
        items.AddRange(itemConsumed);
        craftedObjectRecipe = managerRefs.CraftingManager.PoolCraftedItem(items, out isNew, out tags);

        foreach (TagValue tagValue in tags)
        {
            TagIconUI tagUI = Instantiate(tagUIPrefab, tagUIHolder).GetComponent<TagIconUI>();
            tagUI.Setup(tagValue);
            tagsUI.Add(tagUI);
            tagUI.OnDragReleased += UpdateUIOrder;
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

    //UI Advanced button setup
    public void OnValidateClick()
    {
        CraftedObject craftedObject = managerRefs.CraftingManager.CraftItem(craftedObjectRecipe, items, 0, isNew);
        CurrentCraftingTable.SpawnCraftedItem(craftedObject);
        managerRefs.GameEventsManager.craftEvents.CraftItem(craftedObject.CraftedData);
        managerRefs.UIManager.ToggleCardTagView(false, CurrentCraftingTable);
    }

    private IEnumerator CountScores()
    {
        int score = 0;
        foreach (TagIconUI tagUI in tagsUI)
        {
            score = tagUI.CountTag(score);
            scoreText.text = score.ToString();
            yield return new WaitForSeconds(delayBetweenScoreCount);
        }

        validateButton.SetActive(true);
    }

    private void UpdateUIOrder()
    {
        tagsUI.Sort();
    }
}
