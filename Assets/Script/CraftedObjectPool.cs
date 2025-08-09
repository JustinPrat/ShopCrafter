using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftedObjectPool", menuName = "ShopCrafter/CraftedObjectPool")]
public class CraftedObjectPool : ScriptableObject
{
    public List<CraftedObjectRecipe> craftedObjectPool;

    public CraftedObjectRecipe FindCraftableRecipe(List<Item> toUseItems)
    {
        List<EItemType> usedTypes = new List<EItemType>();
        foreach (Item item in toUseItems)
        {
            usedTypes.Add(item.Type);
        }

        foreach (CraftedObjectRecipe recipe in craftedObjectPool)
        {
            bool canCraft = true;
            foreach (EItemType requiredType in recipe.RequiredItems)
            {
                if (!usedTypes.Contains(requiredType))
                {
                    canCraft = false;
                    break;
                }
            }
            if (canCraft)
            {
                return recipe;
            }
        }
        return null;
    }
}