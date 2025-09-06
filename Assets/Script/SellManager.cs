using System.Collections.Generic;
using UnityEngine;

public class SellManager : MonoBehaviour
{
    [SerializeField] 
    private ManagerRefs managerRefs;

    public List<SellSlot> SellSlots = new List<SellSlot>();
    public List<SellSlot> SellingSlots = new List<SellSlot>();

    private int coinAmount;

    private void Awake()
    {
        managerRefs.SellManager = this;
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

    public void PayForItem (int amount)
    {
        coinAmount = Mathf.Max(coinAmount - amount, 0);
    }

    public SellSlot GetRandomSellSlot ()
    {
        return SellingSlots.GetRandomElement();
    }
}
