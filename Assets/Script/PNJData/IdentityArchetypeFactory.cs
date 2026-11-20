using UnityEngine;

public class IdentityArchetypeFactory : IdentityFactory
{
    private Deck<string> remainingNames = new Deck<string>();
    private Deck<string> remainingDescriptions = new Deck<string>();
    private Deck<Sprite> remainingPortraits = new Deck<Sprite>();
    private Deck<DialogueData> remainingDialogues = new Deck<DialogueData>();

    public IdentityArchetypeFactory(PNJIdentityArchetype data) : base(data)
    {
        remainingNames.Initialize(data.possibleNames);
        remainingDescriptions.Initialize(data.possibleDescriptions);
        remainingPortraits.Initialize(data.possiblePortraits);
        remainingDialogues.Initialize(data.possibleDialogues);
    }

    protected override Identity Pick()
    {
        Identity identity = new Identity()
        {
            Name = remainingNames.Pick(),
            Description = remainingDescriptions.Pick(),
            Portrait = remainingPortraits.Pick(),
            Dialogue = remainingDialogues.Pick()
        };

        return identity;
    }
}