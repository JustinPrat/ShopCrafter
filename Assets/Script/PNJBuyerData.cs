using UnityEngine;

[CreateAssetMenu(fileName = "PNJBuyerData", menuName = "ShopCrafter/PNJBuyerData")]
public class PNJBuyerData : PNJRandomData
{
    public float BuyProbability;

    public override PNJStats GetStats()
    {
        PNJBuyerStats stats = new PNJBuyerStats();
        stats.Name = Name;
        stats.Description = Description;
        stats.ShopStayDuration = ShopStayDuration;
        stats.BuyProbability = BuyProbability;
        return stats;
    }
}

public class PNJBuyerStats : PNJRandomStats
{
    public float BuyProbability;
    public SellSlot Slot;

    public override void OnSpawn(PNJBrain pnjBrain)
    {
        base.OnSpawn(pnjBrain);
        pnjBrain.PNJBuying.Value.Event += OnPnjBuying;

        if (pnjBrain.ManagerRefs.SellManager.IsSellingSlots)
        {
            Slot = pnjBrain.RandomChooseSellSlot();
            pnjBrain.ChangeState(State.Buying);
        }

        Debug.Log("Je suis un acheteur nommé " + Name);
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
