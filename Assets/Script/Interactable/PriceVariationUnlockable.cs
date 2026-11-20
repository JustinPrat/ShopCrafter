public class PriceVariationUnlockable : Unlockable
{
    public override void StartInteract(PlayerBrain playerBrain)
    {
        base.StartInteract(playerBrain);
        managerRefs.GameEventsManager.OnUnlockPriceVariation?.Invoke();
    }
}
