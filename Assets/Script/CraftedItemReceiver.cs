using UnityEngine;

public class CraftedItemReceiver : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private Transform objectHoldAnchor;

    private CraftedObject heldObject;

    public Sprite InteractIcon => icon;
    public CraftedObject HeldObject => heldObject;
    public bool HasHeldItem => heldObject != null;

    public bool CanTakeItem (PlayerBrain brain) => brain.HasItem && !HasHeldItem;
    public bool CanGiveItem (PlayerBrain brain) => !brain.HasItem && HasHeldItem;

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return (playerBrain.HasItem && !HasHeldItem) || (!playerBrain.HasItem && HasHeldItem);
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        if (CanTakeItem(playerBrain))
        {
            heldObject = playerBrain.HeldObject;
            heldObject.transform.SetParent(objectHoldAnchor, false);
            playerBrain.DropItem();
        }
        else if (CanGiveItem(playerBrain))
        {
            transform.SetParent(playerBrain.ObjectHoldAnchor);
            transform.localPosition = Vector3.zero;
            playerBrain.TryHoldItem(heldObject);
        }
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        
    }
}
