using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEditor;
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

    [OnValueChanged(nameof(UpdateCraftedObjectData))]
    public string CraftedName;
    public string CraftedDescription;
    public Sprite CraftedSprite;
    public BarData BarElementData;
    public TierList TierList;
    public int BasePrice;

    [SerializeField]
    private SpawnedReward rewardPrefab;
    public SpawnedReward RewardPrefab { get => rewardPrefab; set => rewardPrefab = value; }

#if UNITY_EDITOR
    private void UpdateCraftedObjectData()
    {
        EditorApplication.delayCall += () =>
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), "Recipe " + CraftedName);
        };
    }
#endif

    public bool CanPay(ManagerRefs managerRefs)
    {
        return managerRefs.PlayerManager.HasCraftedItem(this);
    }

    public void ResolveCost(ManagerRefs managerRefs)
    {
        managerRefs.PlayerManager.ConsumeCraftedItem();
    }

    public ICost.UIDisplayData GetCostDisplayData()
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

    public IRewardable.UIDisplayData GetRewardDisplayData()
    {
        return new IRewardable.UIDisplayData
        {
            DisplayName = CraftedName,
            Icon = CraftedSprite,
            HighlightColor = Rarity.RarityColor
        };
    }
}

[Serializable]
public class CraftedObjectData
{
    [ReadOnly, SerializeField]
    private CraftedObjectRecipe craftedObjectRecipe;
    private ManagerRefs managerRefs;
    private ModifiableValue price = new ModifiableValue();
    private bool isNew;

    public CraftedObjectRecipe CraftedObjectRecipe => craftedObjectRecipe;
    public Rarity Rarity => craftedObjectRecipe.Rarity;
    public bool IsNew => isNew;

    public CraftedObjectData (CraftedObjectRecipe craftedObjectRecipe, ManagerRefs managerRefs, bool isNew)
    {
        this.craftedObjectRecipe = craftedObjectRecipe;
        this.managerRefs = managerRefs;
        this.isNew = isNew;

        //if (craftedObjectRecipe.Rarity.ERarity != ERarity.Unique)
        //{
        //    FindRarity();
        //    SetPrice();
        //}
        //else
        //{
        //    rarity = managerRefs.CraftingManager.RarityHierarchy.UniqueRarity;
        //    basePrice = 0;
        //}
    }

    public int GetPrice()
    {
        price.BaseValue = craftedObjectRecipe.BasePrice * (managerRefs.SellManager.PriceVariations[craftedObjectRecipe.CraftedType].currentPricePercent / 100f);
        return price.Value;
    }

    //public void BoostRarity(int boostRarity)
    //{
    //    if (boostRarity <= 0 || (int)craftedObjectRecipe.Rarity.ERarity + boostedRarity > Enum.GetValues(typeof(ERarity)).Length - 1)
    //        return;

    //    boostedRarity += boostRarity;
    //    FindRarity();
    //    SetPrice();
    //}

    public void AddPriceModifier(StatModifier modifier)
    {
        if (modifier != null)
        {
            price.AddModifier(modifier);
        }
    }

    //public void FindRarity ()
    //{
    //    RarityHierarchy hierarchy = managerRefs.CraftingManager.RarityHierarchy;
    //    rarity = hierarchy.RarityList[Mathf.Min(hierarchy.RarityList.Length -1, (int)craftedObjectRecipe.Rarity.ERarity + boostedRarity)];
    //}

    //private void SetPrice()
    //{
    //    PricePerRarity prices = managerRefs.CraftingManager.BasePrices.PricePerRarities[(int)rarity.ERarity];
    //    basePrice = UnityEngine.Random.Range(prices.MinPrice, prices.MaxPrice + 1);
    //}
}
