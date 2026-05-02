using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField] 
    private Sprite icon;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private Transform UICraftAnchor;

    public Collider Collider => collider;

    [SerializeField]
    private List<CraftedItemReceiver> receiveSlots;

    public Sprite InteractIcon => icon;
    public List<CraftedItemReceiver> ReceiveSlots => receiveSlots;
    public bool IsLocked { get; set; }

    public GameObject GameObject => gameObject;


    public bool CanInteract(PlayerBrain playerBrain)
    {
        foreach (CraftedItemReceiver slots in receiveSlots)
        {
            if (!slots.HasHeldItem)
            {
                return true;
            }
        }
        return false;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleCraftingView(true, this, UICraftAnchor == null ? transform.position : UICraftAnchor.position);
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
    }

    public void SpawnCraftedItem (CraftedObject craftedObject)
    {
        foreach (CraftedItemReceiver itemReceiver in receiveSlots)
        {
            if (!itemReceiver.HasHeldItem)
            {
                itemReceiver.SetItem(craftedObject);
                break;
            }
        }
    }
}
