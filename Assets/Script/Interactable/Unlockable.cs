using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unlockable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Item requiredItem;

    [SerializeField]
    private Sprite interactIcon;

    [SerializeField]
    private Collider2D collider;

    [SerializeField]
    private ManagerRefs refs;

    private bool hasBeenUnlocked;
    private List<IInteractable> interactables;

    public Sprite InteractIcon => interactIcon;

    public Collider2D Collider => collider;
    public GameObject GameObject => gameObject;

    public bool IsLocked { get; set; } = false;

    private void Start()
    {
        interactables = GetComponentsInChildren<IInteractable>().ToList();
        if (interactables.Contains(this))
        {
            interactables.Remove(this);
        }

        SetInteractLock(true);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return refs.CraftingManager.HasItem(requiredItem) && !hasBeenUnlocked;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        if (hasBeenUnlocked)
            return;

        hasBeenUnlocked = true;
        refs.CraftingManager.ConsumeItem(requiredItem);
        SetInteractLock(false);
        collider.enabled = false;
    }

    private void SetInteractLock(bool locking)
    {
        foreach (IInteractable interactable in interactables)
        {
            interactable.IsLocked = locking;
            
            if (interactable.Collider != null)
                interactable.Collider.enabled = !locking;
        }
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
        
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        
    }
}
