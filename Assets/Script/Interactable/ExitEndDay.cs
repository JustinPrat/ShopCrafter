using UnityEngine;

public class ExitEndDay : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Sprite interactIcon;

    [SerializeField]
    private GameObject outObject;

    private bool canInteract;

    public Sprite InteractIcon => interactIcon;

    private void Start()
    {
        managerRefs.GameEventsManager.dayEvents.OnEndDay += OnEndDay;
        managerRefs.GameEventsManager.dayEvents.OnStartDay += OnStartDay;
    }

    private void OnDestroy()
    {
        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.dayEvents.OnEndDay -= OnEndDay;
            managerRefs.GameEventsManager.dayEvents.OnStartDay -= OnStartDay;
        }
    }

    private void OnStartDay()
    {
        canInteract = false;
        outObject.SetActive(false);
    }

    private void OnEndDay()
    {
        canInteract = true;
        outObject.SetActive(true);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return canInteract;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleEndDayView(true);
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
    }
}
