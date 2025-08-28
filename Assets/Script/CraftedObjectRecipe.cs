using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public enum ECraftedType
{
    Utility,
    Weapon,
    Armor,
    Consumable
}

[CreateAssetMenu(fileName = "CraftedObjectRecipe", menuName = "ShopCrafter/CraftedObjectRecipe")]
public class CraftedObjectRecipe : ScriptableObject
{
    public List<EItemType> RequiredItems;
    public ECraftedType CraftedType;
    public string CraftedName;
    public string CraftedDescription;
    public Sprite CraftedSprite;
}

[Serializable]
public class CraftedObjectData
{
    [ReadOnly, SerializeField]
    private CraftedObjectRecipe craftedObjectRecipe;

    [ReadOnly, SerializeField]
    private Rarity rarity;

    private ManagerRefs managerRefs;

    [ReadOnly, SerializeField]
    private int basePrice;

    public CraftedObjectRecipe CraftedObjectRecipe => craftedObjectRecipe;
    public Rarity Rarity => rarity;

    public CraftedObjectData (CraftedObjectRecipe craftedObjectRecipe, ManagerRefs managerRefs, List<Item> items, int boostNumber)
    {
        this.craftedObjectRecipe = craftedObjectRecipe;
        this.managerRefs = managerRefs;
        FindRarity(items, boostNumber);
        SetPrice();
    }

    public int GetPrice()
    {
        // eventualy add price market variation
        return basePrice;
    }

    public void FindRarity (List<Item> items, int boostNumber)
    {
        ERarity minRarity = Enum.GetValues(typeof(ERarity)).Cast<ERarity>().Last();
        foreach (Item item in items)
        {
            if (item.RarityInfos.ERarity < minRarity)
            {
                minRarity = item.RarityInfos.ERarity;
            }
        }

        RarityHierarchy hierarchy = managerRefs.CraftingManager.RarityHierarchy;
        rarity = hierarchy.RarityList[Mathf.Min(hierarchy.RarityList.Length -1, (int)minRarity + boostNumber)];
    }

    private void SetPrice ()
    {
        PricePerRarity prices = managerRefs.CraftingManager.BasePrices.PricePerRarities[(int)rarity.ERarity];
        basePrice = UnityEngine.Random.Range(prices.MinPrice, prices.MaxPrice + 1);
    }
}
