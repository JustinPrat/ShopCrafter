using UnityEngine;
using UnityEngine.Events;

public class StartDayToggle : ToggleActivation
{
    [SerializeField]
    private ManagerRefs managerRefs;

    protected override void Start()
    {
        base.Start();
        managerRefs.GameEventsManager.dayEvents.OnEndDay += OnEndDay;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (managerRefs.GameEventsManager != null)
            managerRefs.GameEventsManager.dayEvents.OnEndDay -= OnEndDay;
    }

    private void OnEndDay()
    {
        Toggle();
    }

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        if (managerRefs.PNJManager.CurrentDayPeriod == PNJManager.DayTime.Morning && !activeState)
        {
            return true;
        }

        return false;
    }

    public override void DoInteract(PlayerBrain playerBrain)
    {
        if (!activeState)
            managerRefs.PNJManager.StartAfternoon();

        base.DoInteract(playerBrain);
    }
}
