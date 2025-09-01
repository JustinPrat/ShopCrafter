using TMPEffects.TMPEvents;
using UnityEngine;

[CreateAssetMenu(fileName = "PNJSellerData", menuName = "ShopCrafter/PNJSellerData")]
public class PNJSellerData : PNJRandomData
{
    public int NumberItemSold;

    public override PNJBehaviour GetStats()
    {
        PNJSellerBehaviour stats = new PNJSellerBehaviour(this);
        return stats;
    }
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

        }
    }
}
