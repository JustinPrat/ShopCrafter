using UnityEngine;

public class SellSlot : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private Sprite icon;

    [SerializeField] 
    private Transform objectHoldAnchor;

    [SerializeField] 
    private ManagerRefs refs;

    private CraftedObject heldObject;

    public Sprite InteractIcon => icon;

    public bool IsSelling => heldObject != null;

    public CraftedObject HeldObject => heldObject;

    private void Start()
    {
        refs.SellManager.SellSlots.Add(this);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return playerBrain.HasItem;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        heldObject = playerBrain.HeldObject;
        heldObject.transform.SetParent(objectHoldAnchor, false);
        playerBrain.DropItem();
    }
}
