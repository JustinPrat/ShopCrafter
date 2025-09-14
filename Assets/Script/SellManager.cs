using Alchemy.Inspector;
using Alchemy.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SellManager : MonoBehaviour
{
    [SerializeField] 
    private ManagerRefs managerRefs;

    [SerializeField]
    private int baseMoney;

    public List<SellSlot> SellSlots = new List<SellSlot>();
    public List<SellSlot> SellingSlots = new List<SellSlot>();

    private int coinAmount;

    [ShowInInspector]
    public int CoinAmount => coinAmount;

    private void Awake()
    {
        managerRefs.SellManager = this;
        coinAmount = baseMoney;
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

    public SellSlot GetRandomSellSlot ()
    {
        return SellingSlots.GetRandomElement();
    }
}
