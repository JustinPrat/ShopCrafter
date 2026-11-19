using System;
using UnityEngine;

public class ExitEndDay : BaseWorldInteract
{
    [SerializeField]
    private GameObject outObject;

    private bool canInteract;

    private void Start()
    {
        managerRefs.GameEventsManager.dayEvents.OnEndDay += OnEndDay;
        managerRefs.GameEventsManager.dayEvents.OnStartDay += OnStartDay;
    }

    private void Awake()
    {
        outObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.dayEvents.OnEndDay -= OnEndDay;
            managerRefs.GameEventsManager.dayEvents.OnStartDay -= OnStartDay;
        }

        base.OnDestroy();
    }

    private void OnStartDay()
    {
        canInteract = false;
        outObject.SetActive(false);
        physicCollider.enabled = false; 
    }

    private void OnEndDay()
    {
        canInteract = true;
        outObject.SetActive(true);
        physicCollider.enabled = true;
    }

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        return canInteract;
    }

    public override void StartInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleEndDayView(true);
        base.StartInteract(playerBrain);
    }
}
