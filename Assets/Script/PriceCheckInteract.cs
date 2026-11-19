public class PriceCheckInteract : BaseWorldInteract
{
    public override bool CanInteract(PlayerBrain playerBrain)
    {
        return false;
    }

    public override void OnInteractRange(PlayerBrain playerBrain)
    {
        if (IsLocked)
            return;

        base.OnInteractRange(playerBrain);
        managerRefs.UIManager.TogglePriceCheckView(true, transform.position);
    }

    public override void OutInteractRange(PlayerBrain playerBrain)
    {
        if (IsLocked)
            return;

        base.OnInteractRange(playerBrain);
        managerRefs.UIManager.TogglePriceCheckView(false);
    }
}
