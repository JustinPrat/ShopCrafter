using UnityEngine;

[CreateAssetMenu(fileName = "PNJSellerData", menuName = "ShopCrafter/PNJSellerData")]
public class PNJSellerData : PNJRandomData
{
    public int NumberItemSold;

    public override PNJStats GetStats()
    {
        PNJSellerStats stats = new PNJSellerStats();
        stats.Name = Name;
        stats.Description = Description;
        stats.ShopStayDuration = ShopStayDuration;
        stats.NumberItemSold = NumberItemSold;
        return stats;
    }
}

public class PNJSellerStats : PNJRandomStats
{
    public float NumberItemSold;
}
