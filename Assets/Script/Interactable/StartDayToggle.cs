using UnityEngine;

public class StartDayToggle : ToggleActivation
{
    [SerializeField]
    private ManagerRefs managerRefs;

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        if (managerRefs.PNJManager.CurrentDayTime == PNJManager.DayTime.Morning && !activeState)
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
