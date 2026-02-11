using UnityEngine;

public class CraftedItemReceiver : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private Transform objectHoldAnchor;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Transform UIStatAnchor;

    private CraftedObject heldObject;

    public Sprite InteractIcon => icon;
    public CraftedObject HeldObject => heldObject;
    public bool HasHeldItem => heldObject != null;

    public bool CanTakeItem (PlayerBrain brain) => brain.Inventory.HasItem && !HasHeldItem;

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return (playerBrain.Inventory.HasItem && !HasHeldItem) || (!playerBrain.Inventory.HasItem && HasHeldItem);
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        if (CanTakeItem(playerBrain))
        {
            heldObject = playerBrain.Inventory.HeldObject;
            heldObject.transform.SetParent(objectHoldAnchor, false);
            playerBrain.Inventory.DropItem();
        }
        else
        {
            if (playerBrain.Inventory.TryTakeItem(heldObject))
            {
                heldObject.transform.SetParent(playerBrain.Inventory.ObjectHoldAnchor);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject = null;
            }
        }
    }

    public void SetItem (CraftedObject craftedObject)
    {
        heldObject = craftedObject;
        heldObject.transform.SetParent(objectHoldAnchor, false);
        heldObject.transform.localPosition = Vector3.zero;
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
        if (HasHeldItem)
        {
            managerRefs.UIManager.ToggleCraftedStatView(true, heldObject.CraftedData, UIStatAnchor.transform.position);
        }
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleCraftedStatView(false);
    }
}
