using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniGameView : UIView
{
    [SerializeField]
    private ToCraftItem toCraftItemHolder;

    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private GameObject targetPrefab;

    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Transform targetHolder;

    [SerializeField]
    private PresetType newItemUIEffect;

    [SerializeField]
    private TextMeshProUGUI barNumberText;

    [SerializeField]
    private Sequencer.Sequencer onBarNumberSetSequence;

    [SerializeField]
    private GameObject gaugeFillPrefab;

    [SerializeField]
    private RectTransform gaugeFillParent;

    [SerializeField]
    private Slider gaugeTag;

    [SerializeField]
    private GameObject finishButton;

    [SerializeField]
    private Sequencer.Sequencer finishSequence;

    [SerializeField]
    private Sequencer.Sequencer resetFinishSequence;

    [SerializeField]
    private RectTransform tutoHolder;

    private CraftedObjectData craftedObjectData;
    private BarBehaviour currentBarBehaviour;
    private List<TagValue> tags;
    private List<TagGaugeUI> tagGaugeUIs = new List<TagGaugeUI>();
    private int tagGameIndex;
    private int score;
    private GameObject animatedTuto;

    public CraftingTable CurrentCraftingTable { get; set; }
    public GameObject TargetPrefab => targetPrefab;
    public Slider ProgressBar => progressBar;

    private void Update()
    {
        if (!gameObject.activeInHierarchy || currentBarBehaviour == null)
            return;

        currentBarBehaviour.OnUpdate();
    }

    public void SetBarNumber(string text)
    {
        barNumberText.text = text;

        if (onBarNumberSetSequence != null)
            onBarNumberSetSequence.StartSequence();
    }

    private StatModifier GetScoreBonusModifier(int score, CraftedObjectRecipe recipe)
    {
        StatModifier modifier = recipe.Rarity.MaxStatModifier.Clone(recipe.Rarity.MaxStatModifier);
        modifier.Value *= 1 + (modifier.Value - 1) * Mathf.Min((float)score / recipe.TargetScore, 1);

        return modifier;
    }

    private void CountScores()
    {
        score = 0;
        foreach (TagValue tag in tags)
        {
            score += Mathf.Clamp(tag.Amount.Value, 0, tag.GaugeValue);
        }
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            finishButton.SetActive(false);
            StartTag(tags[0]);

            resetFinishSequence.StartSequence();
        }
        else
        {
            foreach (TagGaugeUI tagGaugeUI in tagGaugeUIs)
            {
                Destroy(tagGaugeUI.gameObject);
            }

            tagGaugeUIs.Clear();
        }
    }

    public void Setup (CraftedObjectData data, List<TagValue> tags)
    {
        this.tags = new List<TagValue>(tags);
        tagGameIndex = 0;
        craftedObjectData = data;

        toCraftItemHolder.Setup(data.CraftedObjectRecipe);
        toCraftItemHolder.SetShadowFadeRatio(1);

        itemImage.sprite = data.CraftedObjectRecipe.CraftedSprite;

        ComputeWaterFilling(tags, data.CraftedObjectRecipe.TargetScore);

        float currentCursorPos = 0;
        RectTransform gaugeBarRect = gaugeTag.GetComponent<RectTransform>();
        foreach (TagValue tag in this.tags)
        {
            TagGaugeUI tagGaugeUI = Instantiate(gaugeFillPrefab, gaugeFillParent).GetComponent<TagGaugeUI>();
            float size = (float)tag.GaugeValue / craftedObjectData.CraftedObjectRecipe.TargetScore;
            SetSizePosition(tagGaugeUI.Rect, gaugeBarRect, currentCursorPos + size / 2, size);
            tagGaugeUI.Setup(tag);

            currentCursorPos += size;
            tagGaugeUIs.Add(tagGaugeUI);
        }

        gaugeTag.value = currentCursorPos;
    }

    private void StartTag(TagValue tagValue)
    {
        BarData barData = tagValue.Asset.GetBarData(tagValue.Amount.Value);
        currentBarBehaviour = barData.GetBehaviour();
        currentBarBehaviour.OnStart(this);
        tagGaugeUIs[tagGameIndex].SetSelection(true);

        if (barData.AnimatedTuto != null)
            animatedTuto = Instantiate(barData.AnimatedTuto, tutoHolder);
    }

    private void NextTag()
    {
        tagGaugeUIs[tagGameIndex].SetSelection(false);
        tagGameIndex++;
        StartTag(tags[tagGameIndex]);
    }

    public void ValidateTag(float sucessRatio)
    {
        if (animatedTuto != null)
        {
            Destroy(animatedTuto);
            animatedTuto = null;
        }

        ApplySuccessRatio(tags[tagGameIndex], sucessRatio);
        UpdateTagGauge(tags[tagGameIndex]);
        SetBarNumber("0");

        if (tagGameIndex + 1 >= tags.Count)
        {
            StartFinishGame();
        }
        else
        {
            NextTag();
        }

        toCraftItemHolder.SetShadowFadeRatio(1 - ((float)tagGameIndex + 1 / tags.Count));
    }

    private void StartFinishGame()
    {
        if (currentBarBehaviour != null)
            currentBarBehaviour = null;

        CountScores();
        StatModifier modifier = GetScoreBonusModifier(score, craftedObjectData.CraftedObjectRecipe);
        craftedObjectData.AddPriceModifier(modifier);

        finishSequence.StartSequence();
    }

    public void DisplayFinish()
    {
        foreach (TagGaugeUI tagGaugeUI in tagGaugeUIs)
        {
            tagGaugeUI.SetSelection(true);
        }

        SetBarNumber(score + "/" + craftedObjectData.CraftedObjectRecipe.TargetScore);
        finishButton.SetActive(true);

        EventSystem.current.SetSelectedGameObject(finishButton);
    }

    protected override void OnInputDeviceChanged()
    {
        base.OnInputDeviceChanged();

        if (managerRefs.InputManager.IsGamepad && finishButton.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(finishButton);
        }
    }

    public void OnFinishClick()
    {
        CraftedObject craftedObject = managerRefs.CraftingManager.CraftItem(craftedObjectData);
        managerRefs.CraftingManager.AddCraftedRecipe(craftedObjectData.CraftedObjectRecipe);
        CurrentCraftingTable.SpawnCraftedItem(craftedObject);

        managerRefs.UIManager.HideMiniGameView();
        CurrentCraftingTable.ExitInteract();

        if (craftedObjectData.IsNew)
        {
            managerRefs.UIManager.ToggleRewardView(true, craftedObject, newItemUIEffect);
        }
    }

    private void ApplySuccessRatio(TagValue tagValue, float sucessRatio)
    {
        MultiplyModifier tagSuccessModifier = new MultiplyModifier() { Value = sucessRatio };
        tagValue.Amount.AddModifier(tagSuccessModifier);
    }

    public void SetSizePosition(RectTransform rect, RectTransform container, float pos, float size)
    {
        float width = size * container.rect.width;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        float localX = Mathf.LerpUnclamped(container.rect.xMin, container.rect.xMax, pos);
        Vector3 worldPos = container.TransformPoint(new Vector3(localX, container.rect.center.y, 0f));
        rect.position = worldPos;

        rect.SetSiblingIndex(rect.GetSiblingIndex() - 1);
    }

    public RectTransform MakeTarget()
    {
        return Instantiate(targetPrefab, targetHolder).GetComponent<RectTransform>();
    }

    public void InstantiateParticles(GameObject particlePrefab, Vector3 pos)
    {
        GameObject ps = Instantiate(particlePrefab, transform);
        ps.transform.position = pos;
    }

    public void UpdateBarPos(float barPos)
    {
        progressBar.value = barPos;
    }

    private void UpdateTagGauge(TagValue tagValue)
    {
        RectTransform gaugeBarRect = gaugeTag.GetComponent<RectTransform>();
        foreach (TagGaugeUI tagGauge in tagGaugeUIs)
        {
            if (tagGauge.CurrentTag == tagValue)
            {
                tagGauge.UpdateValue();
                break;
            }
        }
    }

    private void ComputeWaterFilling(List<TagValue> tags, int recipeTarget)
    {
        int total = 0;
        for (int i = 0; i < tags.Count; i++)
            total += tags[i].Amount.Value;

        if (total <= recipeTarget)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                tags[i].GaugeValue = tags[i].Amount.Value;
                tags[i].Surplus = 0;
            }
            return;
        }

        List<TagValue> sorted = new List<TagValue>(tags);
        sorted.Sort((a, b) => a.Amount.Value.CompareTo(b.Amount.Value));

        int remainingTarget = recipeTarget;
        int remainingCount = sorted.Count;

        for (int i = 0; i < sorted.Count; i++)
        {
            int fairShare = remainingTarget / remainingCount;
            if (sorted[i].Amount.Value <= fairShare)
            {
                sorted[i].GaugeValue = sorted[i].Amount.Value;
                sorted[i].Surplus = 0;
                remainingTarget -= sorted[i].Amount.Value;
                remainingCount--;
            }
            else
            {
                int remainder = remainingTarget % remainingCount;
                for (int j = i; j < sorted.Count; j++)
                {
                    int extra = j >= sorted.Count - remainder ? 1 : 0;
                    int share = fairShare + extra;
                    sorted[j].GaugeValue = share;
                    sorted[j].Surplus = sorted[j].Amount.Value - share;
                }
                break;
            }
        }
    }
}
