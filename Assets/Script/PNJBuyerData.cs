using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "PNJBuyerData", menuName = "ShopCrafter/PNJBuyerData")]
public class PNJBuyerData : PNJRandomData
{
    public float BaseBuyProbability;
    public List<ECraftedType> CraftedPrefTypes;

    public float MinRollTimeBeforeTryBuy;
    public float MaxRollTimeBeforeTryBuy;

    public float TimeWaitingBeforeTryBuy;
    public Sprite WaitingIcon;

    public override PNJBehaviour GetStats()
    {
        PNJBuyerBehaviour stats = new PNJBuyerBehaviour(this);
        return stats;
    }
}

public class PNJBuyerBehaviour : PNJRandomBehaviour
{
    public SellSlot Slot;
    private PNJBuyerData currentData => (PNJBuyerData)data;
    private float timerBeforeBuying;
    private bool hasBought;

    public PNJBuyerBehaviour(PNJBuyerData data) : base(data)
    {
        this.data = data;
    }

    public override void OnSpawn(PNJBrain pnjBrain)
    {
        base.OnSpawn(pnjBrain);
        pnjBrain.PNJBuying.Value.Event += OnPnjBuying;
        pnjBrain.PNJArriveBuying.Value.Event += OnPnjArriveBuying;

        timerBeforeBuying = Time.time + Random.Range(currentData.MinRollTimeBeforeTryBuy, currentData.MaxRollTimeBeforeTryBuy);
        hasBought = false;
    }

    public override void OnUpdate(PNJBrain pnjBrain)
    {
        base.OnUpdate(pnjBrain);

        pnjBrain.Agent.BlackboardReference.GetVariable<State>("ActualState", out BlackboardVariable<State> state);

        if (Time.time >= timerBeforeBuying && !hasBought && state != State.GoOut)
        {
            hasBought = true;

            Slot = pnjBrain.RandomChooseSellSlot(currentData.CraftedPrefTypes);
            if (Slot != null)
            {
                pnjBrain.SetBuyTime(currentData.TimeWaitingBeforeTryBuy);
                pnjBrain.ChangeState(State.Buying);
            }
        }
    }

    public override void OnDespawn(PNJBrain pnjBrain)
    {
        base.OnDespawn(pnjBrain);
        pnjBrain.PNJBuying.Value.Event -= OnPnjBuying;
    }

    private void OnPnjArriveBuying(GameObject Pnj)
    {
        PNJBrain PNJBrain = Pnj.GetComponent<PNJBrain>();
        PNJBrain.WorldSpeech.DisplaySpeech("<wave>...</wave>");
    }

    private void OnPnjBuying(GameObject Pnj)
    {
        PNJBrain PNJBrain = Pnj.GetComponent<PNJBrain>();

        if (Slot == null) return;
        if (Random.value <= currentData.BaseBuyProbability)
        {
            PNJBrain.ManagerRefs.SellManager.Buy(Slot);
        }
    }
}
