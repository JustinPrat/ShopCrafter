using System.Collections.Generic;
using System.Linq;
using TNRD;
using UnityEngine;

public class Unlockable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private SerializableInterface<ICost> requiredCost;

    [SerializeField]
    private Sprite interactIcon;

    [SerializeField]
    private Collider2D collider;

    [SerializeField]
    private ManagerRefs refs;

    [SerializeField]
    private GameObject deactivateWhenUnlocked;

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
        return requiredCost != null && requiredCost.Value.CanPay(refs) && !hasBeenUnlocked;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        if (hasBeenUnlocked)
            return;

        hasBeenUnlocked = true;
        requiredCost.Value.ResolveCost(refs);
        SetInteractLock(false);
        collider.enabled = false;

        deactivateWhenUnlocked.SetActive(false);
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
