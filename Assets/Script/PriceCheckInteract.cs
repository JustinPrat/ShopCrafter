using System.Collections.Generic;
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

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return false;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
        if (IsLocked)
            return;

        managerRefs.UIManager.TogglePriceCheckView(true, transform.position);
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        if (IsLocked)
            return;

        managerRefs.UIManager.TogglePriceCheckView(false);
    }
}
