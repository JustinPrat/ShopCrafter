public class EncyclopediaInteract : WorldViewInteract<EncyclopedieView>
{
    public override void StartInteract(PlayerBrain playerBrain)
    {
        base.StartInteract(playerBrain);
        managerRefs.UIManager.ToggleEncyclopedieView(true);
    }
}
