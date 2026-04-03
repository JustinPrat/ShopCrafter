using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TagAsset", menuName = "ShopCrafter/Tags/TagAsset")]
public class TagAsset : ScriptableObject
{
    [OnValueChanged(nameof(UpdateTagAssetName))]
    public string Name;

    public string Description;
    public Sprite Icon;
    public List<TagEffect> Effects;

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
}

[Serializable]
public class TagValue
{
    public TagAsset Asset;
    public ModifiableValue Amount;

    public TagValue(TagValue value)
    {
        Asset = value.Asset;
        Amount = new ModifiableValue();
        Amount.BaseValue = value.Amount.BaseValue;
        Amount.StatModifiers = value.Amount.StatModifiers;
    }
}
