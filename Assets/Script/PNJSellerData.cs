using System;
using System.Collections.Generic;
using TMPEffects.TMPEvents;
using UnityEngine;

[CreateAssetMenu(fileName = "PNJSellerData", menuName = "ShopCrafter/PNJSellerData")]
public class PNJSellerData : PNJRandomData
{
    public List<SellingItem> sellingItems;

    public override PNJBehaviour GetStats()
    {
        PNJSellerBehaviour stats = new PNJSellerBehaviour(this);
        return stats;
    }
}

[Serializable]
public class SellingItem
{
    public Item item;
    public int priceEach;
    public int amount;
}

public class PNJSellerBehaviour : PNJRandomBehaviour
{
    private PNJSellerData currentData => (PNJSellerData)data;

    public PNJSellerBehaviour(PNJSellerData data) : base(data)
    {
        this.data = data;
    }

    public override void OnTextEvent(TMPEventArgs args)
    {
        base.OnTextEvent(args);

        if (args.Tag.Name == "shop")
        {
            managerRefs.UIManager.ToggleShopView(true, currentData.sellingItems, this);
        }
    }
}
