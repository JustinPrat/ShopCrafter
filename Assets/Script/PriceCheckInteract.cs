using System.Collections.Generic;
using UnityEngine;

public class PriceCheckInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Sprite icon;

    public Sprite InteractIcon => icon;

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return false;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.TogglePriceCheckView(true, transform.position);
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.TogglePriceCheckView(false);
    }
}
