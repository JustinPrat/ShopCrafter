public class DailyTicketPool
{
    public DailyTicketData Data { get; }
    public float CurrentWeight { get; private set; }
    public int CurrentQuantity { get; private set; }

    protected bool isEmpty = false;
    public virtual bool IsEmpty => isEmpty;

    public DailyTicketPool(DailyTicketData data, float weight, int quantity)
    {
        Data = data;
        CurrentWeight = weight;
        CurrentQuantity = quantity;
    }

    public DailyTicketData.DailyTicketBehavior PickTicket()
    {
        CurrentQuantity -= 1;

        if (CurrentQuantity <= 0)
            isEmpty = true;

        return Pick();
    }

    protected virtual DailyTicketData.DailyTicketBehavior Pick()
    {
        return Data.GetDailyTicketBehavior();
    }
}