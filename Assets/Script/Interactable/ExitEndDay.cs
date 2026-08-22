using System;
using UnityEngine;

public class ExitEndDay : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private string interactText;

    [SerializeField]
    private GameObject outObject;

    [SerializeField]
    private Collider collider;

    private bool canInteract;
    public Collider PhysicCollider => collider;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;
    public bool IsLocked { get; set; }
    public Action<IInteractable> OnDestroyEvent { get; set; }

    private void Start()
    {
        managerRefs.GameEventsManager.dayEvents.OnEndDay += OnEndDay;
        managerRefs.GameEventsManager.dayEvents.OnStartDay += OnStartDay;
    }

    private void Awake()
    {
        outObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.dayEvents.OnEndDay -= OnEndDay;
            managerRefs.GameEventsManager.dayEvents.OnStartDay -= OnStartDay;
        }

        OnDestroyEvent?.Invoke(this);
    }

    private void OnStartDay()
    {
        canInteract = false;
        outObject.SetActive(false);
        collider.enabled = false; 
    }

    private void OnEndDay()
    {
        canInteract = true;
        outObject.SetActive(true);
        collider.enabled = true;
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return canInteract;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleEndDayView(true);
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
