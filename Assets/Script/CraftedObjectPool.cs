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
        values = CraftingManager.CombineItemTags(toUseItems);

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
                    if (tagValue.Asset == requiredTag.Asset && tagValue.Amount.BaseValue >= requiredTag.Amount.BaseValue)
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