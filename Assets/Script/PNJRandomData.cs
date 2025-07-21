public class PNJRandomData : PNJData
{
    public float ShopStayDuration;

    public override PNJStats GetStats()
    {
        PNJRandomStats stats = new PNJRandomStats();
        stats.Name = Name;
        stats.Description = Description;
        stats.ShopStayDuration = ShopStayDuration;
        return stats;
    }
}

public class PNJRandomStats : PNJStats
{
    public float ShopStayDuration;
}
