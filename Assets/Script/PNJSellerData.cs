using UnityEngine;

[CreateAssetMenu(fileName = "PNJSellerData", menuName = "ShopCrafter/PNJSellerData")]
public class PNJSellerData : PNJRandomData
{
    public int NumberItemSold;

    public override PNJStats GetStats()
    {
        PNJSellerStats stats = new PNJSellerStats(this);
        return stats;
    }
}

public class PNJSellerStats : PNJRandomStats
{
    private PNJSellerData data;

    public PNJSellerStats(PNJSellerData data) : base(data)
    {
        this.data = data;
    }
}
