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
public struct SellingItem
{
    public Item item;
    public int priceEach;
    public int amount;
}

public class PNJSellerBehaviour : PNJRandomBehaviour
{
    private PNJSellerData currentData => (PNJSellerData)data;

    private List<SellingItem> sellingItems = new List<SellingItem>();

    public PNJSellerBehaviour(PNJSellerData data) : base(data)
    {
        this.data = data;
        sellingItems.AddRange(data.sellingItems);
    }

    public override void OnTextEvent(TMPEventArgs args)
    {
        base.OnTextEvent(args);

        if (args.Tag.Name == "shop")
        {
            managerRefs.UIManager.ToggleShopView(true, sellingItems, this);
        }
    }

    public override void OnItemBuy(SellingItem clickedItem, ItemShopUI itemShopUI)
    {
        base.OnItemBuy(clickedItem, itemShopUI);
        for (int i = sellingItems.Count - 1; i >= 0; i--)
        {
            SellingItem item = sellingItems[i];

            if (item.item == clickedItem.item)
            {
                item.amount -= 1;
                if (item.amount <= 0)
                {
                    sellingItems.Remove(item);
                }
                else
                {
                    sellingItems[i] = item;
                }

                break;
            }
        }
    }
}
