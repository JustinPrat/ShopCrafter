using UnityEngine;

public class CraftedItemReceiver : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private Transform objectHoldAnchor;

    [SerializeField]
    private ManagerRefs managerRefs;

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
            heldObject.transform.SetParent(playerBrain.ObjectHoldAnchor);
            heldObject.transform.localPosition = Vector3.zero;
            playerBrain.TryHoldItem(heldObject);
            heldObject = null;
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
        managerRefs.UIManager.ToggleCraftedStatView(true, heldObject.CraftedData);
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleCraftedStatView(false);
    }
}
