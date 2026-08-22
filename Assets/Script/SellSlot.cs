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
            sequencer.StartSequence();
    }

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        return base.CanInteract(playerBrain) && (playerBrain.Inventory.HasItem && playerBrain.Inventory.HeldObject.CraftedData.Rarity.ERarity != ERarity.Unique);
    }

    public override void OnInteractRange(PlayerBrain playerBrain)
    {
        base.OnInteractRange(playerBrain);

        if (HasHeldItem)
        {

        }
    }
}
