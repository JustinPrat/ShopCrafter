using System;
using UnityEngine;

public class BaseWorldInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    protected Collider physicCollider;

    [SerializeField]
    protected ManagerRefs managerRefs;

    [SerializeField]
    protected string interactText = "Interact";
    protected bool isInteracting;

    public virtual bool IsLocked { get; set; }
    public virtual string InteractText => interactText;
    public GameObject GameObject => gameObject;
    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }
    public Action<IInteractable> OnStartInteractEvent { get; set; }
    public Action<IInteractable> OnReleaseInteractEvent { get; set; }
    public bool IsInteracting => isInteracting;

    protected virtual void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public virtual bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public virtual void StartInteract(PlayerBrain playerBrain)
    {
        OnStartInteractEvent?.Invoke(this);
    }

    public virtual void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public virtual void OnTargeted(PlayerBrain playerBrain)
    {
        OnTargetedEvent?.Invoke(this);
    }

    public virtual void OutInteractRange(PlayerBrain playerBrain)
    {
    }

    public virtual void UnTargeted(PlayerBrain playerBrain)
    {
        OnUnTargetedEvent?.Invoke(this);
    }

    public virtual void ReleaseInteract(PlayerBrain playerBrain)
    {
        OnReleaseInteractEvent?.Invoke(this);
    }
}
