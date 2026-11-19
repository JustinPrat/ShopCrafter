using UnityEngine;

public class CraftedItemReceiver : BaseWorldInteract
{
    [SerializeField]
    protected bool canTakeBackItem = true;

    [SerializeField]
    protected Transform objectHoldAnchor;

    [SerializeField]
    protected Transform UIStatAnchor;

    protected CraftedObject heldObject;
    public CraftedObject HeldObject => heldObject;
    public bool HasHeldItem => heldObject != null;
    public bool CanTakePlayerItem (PlayerBrain brain) => brain.Inventory.HasItem && !HasHeldItem;

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        return (playerBrain.Inventory.HasItem && !HasHeldItem) || (!playerBrain.Inventory.HasItem && HasHeldItem && canTakeBackItem);
    }

    public override void StartInteract(PlayerBrain playerBrain)
    {
        if (CanTakePlayerItem(playerBrain))
        {
            heldObject = playerBrain.Inventory.HeldObject;
            heldObject.transform.SetParent(objectHoldAnchor, false);
            playerBrain.Inventory.DropItem();
            OnItemReceived();
        }
        else if (HasHeldItem)
        {
            if (playerBrain.Inventory.TryTakeItem(heldObject))
            {
                heldObject.transform.SetParent(playerBrain.Inventory.ObjectHoldAnchor);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject = null;

                OnItemTaken();
            }
        }

        base.StartInteract(playerBrain);
    }

    protected virtual void OnItemReceived() { }
    protected virtual void OnItemTaken() { }

    public virtual void SetItem (CraftedObject craftedObject)
    {
        heldObject = craftedObject;
        heldObject.transform.SetParent(objectHoldAnchor, false);
        heldObject.transform.localPosition = Vector3.zero;
    }

    public override void OnTargeted(PlayerBrain playerBrain)
    {
        OnTargetedEvent?.Invoke(this);

        if (HasHeldItem)
        {
            managerRefs.UIManager.ToggleCraftedStatView(true, heldObject.CraftedData, UIStatAnchor.transform.position);
        }
    }

    public override void UnTargeted(PlayerBrain playerBrain)
    {
        base.UnTargeted(playerBrain);
        managerRefs.UIManager.ToggleCraftedStatView(false);
    }
}
