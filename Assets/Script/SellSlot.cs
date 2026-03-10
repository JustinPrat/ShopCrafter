using UnityEngine;

public class SellSlot : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private Sprite icon;

    [SerializeField]
    private Transform objectHoldAnchor;

    [SerializeField] 
    private ManagerRefs refs;

    [SerializeField]
    private Collider2D collider;
    public Collider2D Collider => collider;
    public GameObject GameObject => gameObject;

    private CraftedObject heldObject;

    public Sprite InteractIcon => icon;

    public bool IsSelling => heldObject != null;

    public CraftedObject HeldObject => heldObject;
    public bool IsLocked { get; set; }


    private void Start()
    {
        refs.SellManager.OnItemRemoved(this);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return playerBrain.Inventory.HasItem && !IsSelling;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        heldObject = playerBrain.Inventory.HeldObject;
        heldObject.transform.SetParent(objectHoldAnchor, false);
        refs.SellManager.OnItemSelling(this);
        playerBrain.Inventory.DropItem();
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }
}
