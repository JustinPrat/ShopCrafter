using UnityEngine;

[CreateAssetMenu(fileName = "PNJBuyerData", menuName = "ShopCrafter/PNJBuyerData")]
public class PNJBuyerData : PNJRandomData
{
    public float BuyProbability;

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

    public PNJBuyerBehaviour(PNJBuyerData data) : base(data)
    {
        this.data = data;
    }

    public override void OnSpawn(PNJBrain pnjBrain)
    {
        base.OnSpawn(pnjBrain);
        pnjBrain.PNJBuying.Value.Event += OnPnjBuying;

        if (pnjBrain.ManagerRefs.SellManager.IsSellingSlots)
        {
            Slot = pnjBrain.RandomChooseSellSlot();
            pnjBrain.ChangeState(State.Buying);
        }

        Debug.Log("Je suis un acheteur nommé " + data.Name);
    }

    public override void OnDespawn(PNJBrain pnjBrain)
    {
        base.OnDespawn(pnjBrain);
        pnjBrain.PNJBuying.Value.Event -= OnPnjBuying;
    }

    private void OnPnjBuying(GameObject Pnj)
    {
        if (Slot == null) return;
        PNJBrain PNJBrain = Pnj.GetComponent<PNJBrain>();
        PNJBrain.ManagerRefs.SellManager.Buy(Slot);
    }
}
