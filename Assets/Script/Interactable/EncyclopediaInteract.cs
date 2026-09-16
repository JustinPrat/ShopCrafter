using System;
using UnityEngine;

public class EncyclopediaInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private ManagerRefs managerRefs;

    public bool IsLocked { get; set; }

    public string InteractText => "Encyclopedia";

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleEncyclopedieView(true);
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
        OnTargetedEvent?.Invoke(this);
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
        OnUnTargetedEvent?.Invoke(this);
    }
}
