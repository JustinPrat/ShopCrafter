using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private string interactText;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private Transform UICraftAnchor;

    public Collider PhysicCollider => collider;

    [SerializeField]
    private List<CraftedItemReceiver> receiveSlots;

    public List<CraftedItemReceiver> ReceiveSlots => receiveSlots;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;
    public bool IsLocked { get; set; }

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
