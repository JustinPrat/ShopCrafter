using UnityEngine;

public class CraftedItemReceiver : MonoBehaviour, IInteractable
{
    [SerializeField]
    protected bool canTakeBackItem = true;

    [SerializeField]
    protected string interactText;

    [SerializeField]
    protected Transform objectHoldAnchor;

    [SerializeField]
    protected ManagerRefs managerRefs;

    [SerializeField]
    protected Transform UIStatAnchor;

    [SerializeField]
    protected Collider physicCollider;

    protected CraftedObject heldObject;

    public CraftedObject HeldObject => heldObject;
    public bool HasHeldItem => heldObject != null;
    public Collider PhysicCollider => physicCollider;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;

    public bool IsLocked { get; set; }

    public bool CanTakePlayerItem (PlayerBrain brain) => brain.Inventory.HasItem && !HasHeldItem;

    public virtual bool CanInteract(PlayerBrain playerBrain)
    {
        return (playerBrain.Inventory.HasItem && !HasHeldItem) || (!playerBrain.Inventory.HasItem && HasHeldItem && canTakeBackItem);
    }

    public virtual void DoInteract(PlayerBrain playerBrain)
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
    }

    protected virtual void OnItemReceived() { }
    protected virtual void OnItemTaken() { }

    public virtual void SetItem (CraftedObject craftedObject)
    {
        heldObject = craftedObject;
        heldObject.transform.SetParent(objectHoldAnchor, false);
        heldObject.transform.localPosition = Vector3.zero;
    }

    public virtual void OnInteractRange(PlayerBrain playerBrain)
    {
        if (HasHeldItem)
        {
            managerRefs.UIManager.ToggleCraftedStatView(true, heldObject.CraftedData, UIStatAnchor.transform.position);
        }
    }

    public virtual void OutOfInteractRange(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleCraftedStatView(false);
    }
}
