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
public class CraftedObjectRecipe : ScriptableObject, IRewardable, ICost
{
    public List<TagValue> RequiredTags;
    public ECraftedType CraftedType;
    public Rarity Rarity;
    public int TargetScore;

    public string CraftedName;
    public string CraftedDescription;
    public Sprite CraftedSprite;

    public bool CanPay(ManagerRefs managerRefs)
    {
        return managerRefs.PlayerManager.HasCraftedItem(this);
    }

    public void ResolveCost(ManagerRefs managerRefs)
    {
        managerRefs.PlayerManager.ConsumeCraftedItem();
    }

    public ICost.UIDisplayData GetDisplayData()
    {
        return new ICost.UIDisplayData
        {
            DisplayName = CraftedName,
            Amount = 1,
            Icon = CraftedSprite
        };
    }

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

    private ModifiableValue price = new ModifiableValue();
    private int boostedRarity;
    private bool isNew;

    public CraftedObjectRecipe CraftedObjectRecipe => craftedObjectRecipe;
    public Rarity Rarity => rarity;
    public bool IsNew => isNew;

    public CraftedObjectData (CraftedObjectRecipe craftedObjectRecipe, ManagerRefs managerRefs, bool isNew)
    {
        this.craftedObjectRecipe = craftedObjectRecipe;
        this.managerRefs = managerRefs;
        this.isNew = isNew;
        FindRarity();
        SetPrice();
    }

    public int GetPrice()
    {
        price.BaseValue = basePrice * (managerRefs.SellManager.PriceVariations[craftedObjectRecipe.CraftedType].currentPricePercent / 100f);
        return price.Value;
    }

    public void BoostRarity(int boostRarity)
    {
        if (boostRarity <= 0 || (int)craftedObjectRecipe.Rarity.ERarity + boostedRarity > Enum.GetValues(typeof(ERarity)).Length - 1)
            return;

        boostedRarity += boostRarity;
        FindRarity();
        SetPrice();
    }

    public void AddModifier(StatModifier modifier)
    {
        if (modifier != null)
        {
            price.AddModifier(modifier);
        }
    }

    public void FindRarity ()
    {
        RarityHierarchy hierarchy = managerRefs.CraftingManager.RarityHierarchy;
        rarity = hierarchy.RarityList[Mathf.Min(hierarchy.RarityList.Length -1, (int)craftedObjectRecipe.Rarity.ERarity + boostedRarity)];
    }

    private void SetPrice ()
    {
        PricePerRarity prices = managerRefs.CraftingManager.BasePrices.PricePerRarities[(int)rarity.ERarity + boostedRarity];
        basePrice = UnityEngine.Random.Range(prices.MinPrice, prices.MaxPrice + 1);
    }
}
