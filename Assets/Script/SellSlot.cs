using UnityEngine;

public class SellSlot : CraftedItemReceiver
{
    [SerializeField]
    private Sequencer.Sequencer sequencer;

    private void Start()
    {
        managerRefs.SellManager.OnItemRemoved(this);
    }

    protected override void OnItemReceived()
    {
        base.OnItemReceived();
        managerRefs.SellManager.OnItemSelling(this);

        if (sequencer != null)
        {
            sequencer.StartSequence();
        }
    }
}
