using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField] 
    private Sprite icon;

    [SerializeField]
    private List<CraftedItemReceiver> receiveSlots;

    public Sprite InteractIcon => icon;
    public List<CraftedItemReceiver> ReceiveSlots => receiveSlots;

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
        managerRefs.UIManager.ToggleCraftingView(true, this, transform.position);
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
