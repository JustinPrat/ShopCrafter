using UnityEngine;

public class SellSlot : CraftedItemReceiver
{
    [SerializeField]
    private Sequencer.Sequencer sequencer;

    [SerializeField]
    private Transform buyPosition;

    public Transform BuyPosition => buyPosition;

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

    public override void DoInteract(PlayerBrain playerBrain)
    {
        base.DoInteract(playerBrain);
        OnInteractRange(playerBrain);
    }

    public override void OnInteractRange(PlayerBrain playerBrain)
    {
        base.OnInteractRange(playerBrain);
        if (HasHeldItem)
        {
            managerRefs.UIManager.ToggleCraftedStatView(true, heldObject.CraftedData, UIStatAnchor.transform.position);
        }
    }

    public override void OutInteractRange(PlayerBrain playerBrain)
    {
        base.OutInteractRange(playerBrain);
        managerRefs.UIManager.ToggleCraftedStatView(false);
    }

    public override void OnTargeted(PlayerBrain playerBrain)
    {
    }

    public override void UnTargeted(PlayerBrain playerBrain)
    {
    }
}
