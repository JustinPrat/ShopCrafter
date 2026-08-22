using System;
using System.Collections.Generic;
using System.Linq;
using TNRD;
using UnityEngine;

public class Unlockable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private SerializableInterface<ICost> requiredCost;

    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private string interactText;

    [SerializeField]
    private ManagerRefs refs;

    [SerializeField]
    private Sequencer.Sequencer sequenceOnUnlocked;

    private bool hasBeenUnlocked;
    private List<IInteractable> interactables;

    public Collider PhysicCollider => physicCollider;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;
    public bool IsLocked { get; set; } = false;
    public Action<IInteractable> OnDestroyEvent { get; set; }

    private void Start()
    {
        interactables = GetComponentsInChildren<IInteractable>().ToList();
        if (interactables.Contains(this))
        {
            interactables.Remove(this);
        }

        SetInteractLock(true);
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
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
        physicCollider.enabled = false;

        if (sequenceOnUnlocked != null)
            sequenceOnUnlocked.StartSequence();
    }

    private void SetInteractLock(bool locking)
    {
        foreach (IInteractable interactable in interactables)
        {
            interactable.IsLocked = locking;
            
            if (interactable.PhysicCollider != null)
                interactable.PhysicCollider.enabled = !locking;
        }
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
        
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
        
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }
}
