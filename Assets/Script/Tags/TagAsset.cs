using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TagAsset", menuName = "ShopCrafter/Tags/TagAsset")]
public class TagAsset : ScriptableObject
{
#if UNITY_EDITOR
    [OnValueChanged(nameof(UpdateTagAssetName))]
#endif
    public string Name;

    public string Description;
    public Sprite Icon;
    public Color TagOnGaugeColor = Color.white;
    public Color TagOffGaugeColor = Color.white;
    public List<TagEffect> Effects;
    public List<BarData> BarElementDatas;

#if UNITY_EDITOR
    private void UpdateTagAssetName()
    {
        EditorApplication.delayCall += () =>
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), "Tag " + Name);
        };
    }
#endif

    public int ApplyTagAsset(int score)
    {
        foreach (TagEffect effect in Effects)
        {
            score = effect.ApplyTagEffect(score);
        }

        return score;
    }

    public void PreSelectionApplyTagAsset(List<TagValue> tagValues, int index)
    {
        foreach (TagEffect effect in Effects)
        {
            effect.PreSelectionApply(tagValues, index);
        }
    }

    public void PreSelectionRemoveTagAsset(List<TagValue> tagValues, int index)
    {
        foreach (TagEffect effect in Effects)
        {
            effect.PreSelectionRemove(tagValues, index);
        }
    }

    public BarData GetBarData(int tagAmount)
    {
        BarData selectedBar = null;
        foreach (BarData barData in BarElementDatas)
        {
            if (barData.MinTagAmount <= tagAmount)
            {
                if (selectedBar == null || barData.MinTagAmount > selectedBar.MinTagAmount)
                {
                    selectedBar = barData;
                }
            }
        }

        if (selectedBar == null && BarElementDatas.Count > 0)
            selectedBar = BarElementDatas[0];

        return selectedBar;
    }
}

[Serializable]
public class TagValue
{
    public TagAsset Asset;
    public ModifiableValue Amount;

    public int GaugeValue;
    public int Surplus;

    public TagValue(TagValue value)
    {
        Asset = value.Asset;
        Amount = new ModifiableValue();
        Amount.BaseValue = value.Amount.BaseValue;
        Amount.StatModifiers = value.Amount.StatModifiers;
    }
}
