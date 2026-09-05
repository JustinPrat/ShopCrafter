using System;
using UnityEngine;

public class PriceCheckInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private string interactText;

    [SerializeField]
    private Collider collider;

    public bool IsLocked { get; set; }
    public Collider PhysicCollider => collider;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;

    public Action<IInteractable> OnDestroyEvent { get; set; }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return false;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
        if (IsLocked)
            return;

        managerRefs.UIManager.TogglePriceCheckView(true, transform.position);
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
        if (IsLocked)
            return;

        managerRefs.UIManager.TogglePriceCheckView(false);
    }
}
