public class PriceVariationUnlockable : Unlockable
{
    public override void DoInteract(PlayerBrain playerBrain)
    {
        base.DoInteract(playerBrain);
        refs.GameEventsManager.OnUnlockPriceVariation?.Invoke();
    }
}
