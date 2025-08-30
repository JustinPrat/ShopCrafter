public class PNJRandomData : PNJData
{
    public float ShopStayDuration;
    public DialogueData PNJDialogueData;

    public override PNJStats GetStats()
    {
        PNJRandomStats stats = new PNJRandomStats(this);
        return stats;
    }
}

public class PNJRandomStats : PNJStats
{
    private PNJRandomData data;

    public PNJRandomStats(PNJRandomData data) : base(data)
    {
        this.data = data;
    }

    public override void OnInteract(PNJBrain pnjBrain)
    {
        base.OnInteract(pnjBrain);
        data.ManagerRefs.DialogueManager.StartDialogue(data.PNJDialogueData, data);
    }
}
