using System;
using System.Collections.Generic;

public class DailyTicketPool<T> : DailyTicketPool
{
    private List<T> remainingItems;
    private Func<T, DailyTicketData.DailyTicketBehavior> behaviorFactory;

    public override bool IsEmpty => remainingItems.Count == 0 || isEmpty;

    public DailyTicketPool(DailyTicketData data, float weight, int quantity, IEnumerable<T> sourceCollection, Func<T, DailyTicketData.DailyTicketBehavior> behaviorFactory) : base(data, weight, quantity)
    {
        remainingItems = new List<T>(sourceCollection);
        this.behaviorFactory = behaviorFactory;
    }

    protected override DailyTicketData.DailyTicketBehavior Pick()
    {
        int index = UnityEngine.Random.Range(0, remainingItems.Count);
        T pickedItem = remainingItems[index];

        remainingItems.RemoveAt(index);

        return behaviorFactory.Invoke(pickedItem);
    }
}
