using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagAsset", menuName = "ShopCrafter/Tags/TagAsset")]
public class TagAsset : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public List<TagEffect> Effects;

    public int ApplyTagAsset(int score)
    {
        foreach (TagEffect effect in Effects)
        {
            score = effect.ApplyTagEffect(score);
        }

        return score;
    }
}

[Serializable]
public class TagValue
{
    public TagAsset Asset;
    public int Amount;

    public TagValue(TagValue value)
    {
        Asset = value.Asset;
        Amount = value.Amount;
    }
}
