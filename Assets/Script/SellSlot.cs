using UnityEngine;

public class SellSlot : CraftedItemReceiver
{
    private void Start()
    {
        managerRefs.SellManager.OnItemRemoved(this);
    }

    protected override void OnItemReceived()
    {
        base.OnItemReceived();
        managerRefs.SellManager.OnItemSelling(this);
    }
}
