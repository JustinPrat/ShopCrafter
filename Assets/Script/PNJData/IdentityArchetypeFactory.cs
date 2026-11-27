public class IdentityArchetypeFactory : IdentityFactory
{
    private Deck<string> remainingNames = new Deck<string>();
    private Deck<string> remainingDescriptions = new Deck<string>();
    private Deck<SpriteController> remainingVisuals = new Deck<SpriteController>();
    private Deck<DialogueData> remainingDialogues = new Deck<DialogueData>();

    public IdentityArchetypeFactory(PNJIdentityArchetype data) : base(data)
    {
        remainingNames.Initialize(data.possibleNames);
        remainingDescriptions.Initialize(data.possibleDescriptions);
        remainingVisuals.Initialize(data.possibleVisuals);
        remainingDialogues.Initialize(data.possibleDialogues);
    }

    protected override Identity Pick()
    {
        SpriteController spriteController = remainingVisuals.Pick();
        Identity identity = new Identity()
        {
            Name = remainingNames.Pick(),
            Description = remainingDescriptions.Pick(),
            Portrait = spriteController.Sprite,
            Controller = spriteController.Controller,
            Dialogue = remainingDialogues.Pick()
        };

        return identity;
    }
}