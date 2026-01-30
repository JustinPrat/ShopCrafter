using System;
using System.Collections.Generic;
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

    public SellerRuntime(SellerData data)
    {
        this.data = data;
        currentInventory = new List<SellingItem>(data.InitialStock);
    }

    public void OnSpawn(PNJBrain pnjBrain)
    {
    }

    public void OnUpdate(PNJBrain pnjBrain) { }

    public void OnDespawn(PNJBrain pnjBrain) { }

    public void OpenShop(PNJBrain brain)
    {
        brain.ManagerRefs.UIManager.ToggleShopView(true, currentInventory, null);
    }
}