using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public enum ECraftedType
{
    Weapon,
    Armor,
    Utility
}

[CreateAssetMenu(fileName = "CraftedObjectRecipe", menuName = "ShopCrafter/CraftedObjectRecipe")]
[Serializable]
public class CraftedObjectRecipe : ScriptableObject, IRewardable
{
    public List<ItemType> RequiredItems;
    public ECraftedType CraftedType;
    public string CraftedName;
    public string CraftedDescription;
    public Sprite CraftedSprite;
    public BarData BarDataElement;

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver)
    {
        managerRefs.CraftingManager.AddBlueprint(this);
    }
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

    private bool isNew;

    public CraftedObjectRecipe CraftedObjectRecipe => craftedObjectRecipe;
    public Rarity Rarity => rarity;
    public bool IsNew => isNew;

    public CraftedObjectData (CraftedObjectRecipe craftedObjectRecipe, ManagerRefs managerRefs, List<Item> items, int boostNumber, bool isNew)
    {
        this.craftedObjectRecipe = craftedObjectRecipe;
        this.managerRefs = managerRefs;
        this.isNew = isNew;
        FindRarity(items, boostNumber);
        SetPrice();
    }

    public int GetPrice()
    {
        return (int) (basePrice * (managerRefs.SellManager.PriceVariations[craftedObjectRecipe.CraftedType].currentPricePercent / 100f));
    }

    public void FindRarity (List<Item> items, int boostNumber)
    {
        ERarity minRarity = Enum.GetValues(typeof(ERarity)).Cast<ERarity>().First();
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
