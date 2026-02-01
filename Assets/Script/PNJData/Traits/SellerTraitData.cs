using System;
using System.Collections.Generic;
using TMPEffects.TMPEvents;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/Traits/Seller Trait")]
public class SellerTraitData : PNJTraitData
{
    public SellerData Data;

    public override IPNJTraitRuntime GetRuntimeLogic()
    {
        return new SellerRuntime(Data);
    }
}

[Serializable]
public struct SellerData
{
    public List<SellingItem> InitialStock;
}

public class SellerRuntime : IPNJTraitRuntime
{
    private SellerData data;
    private List<SellingItem> currentInventory;
    private PNJBrain pnjBrain;

    public SellerData Data => data;

    public SellerRuntime(SellerData data)
    {
        this.data = data;
        currentInventory = new List<SellingItem>(data.InitialStock);
    }

    public void OnTextEvent(TMPEventArgs args)
    {
        if (args.Tag.Name == TMPEvents.Shop.ToString())
        {
            pnjBrain.ManagerRefs.UIManager.ToggleShopView(true, currentInventory, this, pnjBrain);
        }
    }

    public void OnSpawn(PNJBrain pnjBrain)
    {
        this.pnjBrain = pnjBrain;
    }

    public void OnItemBuy(SellingItem clickedItem, ItemShopUI itemShopUI)
    {
        for (int i = currentInventory.Count - 1; i >= 0; i--)
        {
            SellingItem item = currentInventory[i];

            if (item.item == clickedItem.item)
            {
                item.amount -= 1;
                if (item.amount <= 0)
                {
                    currentInventory.RemoveAt(i);
                }
                else
                {
                    currentInventory[i] = item;
                }

                break;
            }
        }
    }

    public void OnUpdate(PNJBrain pnjBrain) { }

    public void OnDespawn(PNJBrain pnjBrain) { }

    public void OnInteract(PNJBrain pnjBrain) { }
}