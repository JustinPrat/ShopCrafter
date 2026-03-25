using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftedObjectPool", menuName = "ShopCrafter/CraftedObjectPool")]
public class CraftedObjectPool : ScriptableObject
{
    public List<CraftedObjectRecipe> craftedObjectPool;

    public CraftedObjectRecipe FindCraftableRecipe(List<Item> toUseItems)
    {
        return FindCraftableRecipe(toUseItems, out List<TagValue> values);
    }

    public CraftedObjectRecipe FindCraftableRecipe(List<Item> toUseItems, out List<TagValue> values)
    {
        List<EItemType> usedTypes = new List<EItemType>();
        values = new List<TagValue>();
        foreach (Item item in toUseItems)
        {
            usedTypes.Add(item.Type);

            foreach (TagValue tagValue in item.Tags)
            {
                bool hasTag = false;
                foreach (TagValue tagTemp in values)
                {
                    if (tagTemp.Asset == tagValue.Asset)
                    {
                        tagTemp.Amount += tagValue.Amount;
                        hasTag = true;
                        break;
                    }
                }

                if (!hasTag)
                {
                    values.Add(new TagValue(tagValue));
                }
            }
        }

        ERarity highestRarity = ERarity.Common;
        CraftedObjectRecipe targetRecipe = null;
        foreach (CraftedObjectRecipe recipe in craftedObjectPool)
        {
            bool canCraft = true;
            foreach (TagValue requiredTag in recipe.RequiredTags)
            {
                bool hasRequiredTag = false;
                foreach (TagValue tagValue in values)
                {
                    if (tagValue.Asset == requiredTag.Asset && tagValue.Amount >= requiredTag.Amount)
                    {
                        //Match the tag
                        hasRequiredTag = true;
                        break;
                    }
                }

                if (!hasRequiredTag)
                    canCraft = false;
                break;
            }

            if (canCraft && (recipe == null || recipe.Rarity.ERarity > highestRarity))
            {
                highestRarity = recipe.Rarity.ERarity;
                targetRecipe = recipe;
            }
        }

        if (targetRecipe != null)
        {
            return targetRecipe;
        }

        return null;
    }
}