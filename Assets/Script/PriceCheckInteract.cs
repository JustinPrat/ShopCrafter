using System.Collections.Generic;
using UnityEngine;

public class PriceCheckInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private Collider2D collider;

    public Sprite InteractIcon => icon;
    public bool IsLocked { get; set; }
    public Collider2D Collider => collider;
    public GameObject GameObject => gameObject;

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
