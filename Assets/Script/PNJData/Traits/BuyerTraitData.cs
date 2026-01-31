using System;
using System.Collections.Generic;
using TMPEffects.TMPEvents;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/Traits/Buyer Trait")]
public class BuyerTraitData : PNJTraitData
{
    public BuyerData BuyerData;

    public override IPNJTraitRuntime GetRuntimeLogic()
    {
        return new BuyerRuntime(BuyerData);
    }
}

[Serializable]
public struct BuyerData
{
    [Header("Settings")]
    public float BaseBuyProbability;
    public List<ECraftedType> CraftedPrefTypes;
    public float TimeBeforeTryBuy;
    public float TimeWaitingAtTryBuy;
    public Sprite WaitingIcon;
}

public class BuyerRuntime : IPNJTraitRuntime
{
    private BuyerData data;
    private float timerBeforeBuying;
    private bool hasBought;
    private SellSlot targetSlot;

    public BuyerRuntime(BuyerData data)
    {
        this.data = data;
    }

    public void OnSpawn(PNJBrain pnjBrain)
    {
        pnjBrain.PNJBuying.Value.Event += (go) => OnPnjBuying(go, pnjBrain);
        pnjBrain.PNJArriveBuying.Value.Event += (go) => OnPnjArriveBuying(go, pnjBrain);

        timerBeforeBuying = Time.time + data.TimeBeforeTryBuy;
        hasBought = false;
    }

    public void OnUpdate(PNJBrain pnjBrain)
    {
        if (Time.time >= timerBeforeBuying && !hasBought)
        {
            hasBought = true;
            targetSlot = pnjBrain.RandomChooseSellSlot(data.CraftedPrefTypes);

            if (targetSlot != null)
            {
                pnjBrain.SetBuyTime(data.TimeWaitingAtTryBuy);
                pnjBrain.ChangeState(State.Buying);
            }
        }
    }

    public void OnDespawn(PNJBrain pnjBrain)
    {
    }

    private void OnPnjArriveBuying(GameObject go, PNJBrain brain)
    {
        brain.ChangeIcon(data.WaitingIcon);
    }

    private void OnPnjBuying(GameObject go, PNJBrain brain)
    {
        brain.ChangeIcon(null);
        if (targetSlot == null) return;

        if (UnityEngine.Random.value <= data.BaseBuyProbability)
        {
            brain.ManagerRefs.SellManager.Buy(targetSlot);
        }
    }

    public void OnInteract(PNJBrain pnjBrain) { }

    public void OnTextEvent(TMPEventArgs args) { }
    
    public void OnItemBuy(SellingItem clickedItem, ItemShopUI itemShopUI) { }
}