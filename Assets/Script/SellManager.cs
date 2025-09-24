using Alchemy.Inspector;
using Alchemy.Serialization;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[AlchemySerialize]
public partial class SellManager : MonoBehaviour
{
    [SerializeField] 
    private ManagerRefs managerRefs;

    [SerializeField]
    private int baseMoney;

    [SerializeField]
    private int numberToTakeForPriceVariation;

    [NonSerialized, AlchemySerializeField]
    private Dictionary<ECraftedType, PriceVariation> priceVariations;

    [SerializeField]
    private float timeBeforePriceVariation;

    public List<SellSlot> SellSlots = new List<SellSlot>();
    public List<SellSlot> SellingSlots = new List<SellSlot>();

    public Dictionary<ECraftedType, PriceVariation> PriceVariations => priceVariations;

    private int coinAmount;
    private Queue<ECraftedType> priceVariationLastTypes = new Queue<ECraftedType>();

    private float nextTimePriceVariation;

    private void OnValidate()
    {
        EditorUtility.SetDirty(this);
    }

    public class PriceVariation
    {
        public float minVariationPercent;
        public float maxVariationPercent;
        public float percentDecreasePerCraft;

        [DisableInEditMode, DisableInPlayMode]
        public float currentPricePercent;

        [DisableInEditMode, DisableInPlayMode]
        public float BaseRolledPricePercent;

        [DisableInEditMode, DisableInPlayMode]
        public int currentCraftedCount;
    }

    [ShowInInspector]
    public int CoinAmount => coinAmount;

    private void Awake()
    {
        managerRefs.SellManager = this;
        coinAmount = baseMoney;
        UpdatePrices(true);
    }

    [Button]
    private void Add100Coins ()
    {
        coinAmount += 100;
    }

    public bool IsSellingSlots => SellingSlots.Count > 0;

    public void OnItemSelling (SellSlot sellSlot)
    {
        SellSlots.Remove(sellSlot);
        SellingSlots.Add(sellSlot);

        if (priceVariationLastTypes.Count >= numberToTakeForPriceVariation)
        {
            priceVariationLastTypes.Dequeue();
        }

        priceVariationLastTypes.Enqueue(sellSlot.HeldObject.CraftedObjectData.CraftedObjectRecipe.CraftedType);
        UpdatePrices(false);
    }

    public void OnItemRemoved (SellSlot sellSlot)
    {
        SellSlots.Add(sellSlot);
        SellingSlots.Remove(sellSlot);
    }

    public void Buy (SellSlot sellSlot)
    {
        if (sellSlot.IsSelling)
        {
            OnItemSelling(sellSlot);
            coinAmount += sellSlot.HeldObject.Price;
            Destroy(sellSlot.HeldObject.gameObject);
        }
    }

    public bool TryPayForItem (int amount)
    {
        if (coinAmount >= amount)
        {
            coinAmount -= amount;
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (Time.time >= nextTimePriceVariation)
        {
            UpdatePrices(true);
        }
    }

    private void UpdatePrices (bool needReroll)
    {
        nextTimePriceVariation = Time.time + timeBeforePriceVariation;

        foreach (ECraftedType variationType in priceVariationLastTypes)
        {
            priceVariations[variationType].currentCraftedCount += 1;
        }

        foreach (KeyValuePair<ECraftedType, PriceVariation> priceVariation in priceVariations)
        {
            if (needReroll)
            {
                priceVariation.Value.BaseRolledPricePercent = UnityEngine.Random.Range(priceVariation.Value.minVariationPercent, priceVariation.Value.maxVariationPercent);
            }

            priceVariation.Value.currentPricePercent = priceVariation.Value.BaseRolledPricePercent - (priceVariation.Value.percentDecreasePerCraft * priceVariation.Value.currentCraftedCount);
        }

        foreach (KeyValuePair<ECraftedType, PriceVariation> priceVariation in priceVariations)
        {
            priceVariation.Value.currentCraftedCount = 0;
        }
    }

    public SellSlot GetRandomSellSlot ()
    {
        return SellingSlots.GetRandomElement();
    }
}
