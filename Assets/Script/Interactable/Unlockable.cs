using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TNRD;
using UnityEngine;

public class Unlockable : BaseWorldInteract
{
    [SerializeField]
    private SerializableInterface<ICost> requiredCost;

    [SerializeField]
    private Sequencer.Sequencer sequenceOnUnlocked;

    [SerializeField]
    private float waitBeforeEnableInteraction = 0;

    private bool hasBeenUnlocked;
    private List<IInteractable> interactables;

    private void Start()
    {
        interactables = GetComponentsInChildren<IInteractable>().ToList();
        if (interactables.Contains(this))
        {
            interactables.Remove(this);
        }

        SetInteractLock(true);
    }

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        return requiredCost != null && requiredCost.Value.CanPay(managerRefs) && !hasBeenUnlocked;
    }

    public override void StartInteract(PlayerBrain playerBrain)
    {
        if (hasBeenUnlocked)
            return;

        hasBeenUnlocked = true;
        requiredCost.Value.ResolveCost(managerRefs);
        physicCollider.enabled = false;

        if (waitBeforeEnableInteraction <= 0)
        {
            SetInteractLock(false);
        }
        else
        {
            StartCoroutine(WaitBeforeEnableInteraction());
        }

        if (sequenceOnUnlocked != null)
            sequenceOnUnlocked.StartSequence();

        OnStartInteractEvent?.Invoke(this);
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

    private IEnumerator WaitBeforeEnableInteraction()
    {
        yield return new WaitForSeconds(waitBeforeEnableInteraction);
        SetInteractLock(false);
    }
}
