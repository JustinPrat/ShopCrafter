using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/PNJ identity archetype")]
public class PNJIdentityArchetype : IdentityData
{
    public List<string> possibleNames;
    public List<string> possibleDescriptions;
    public List<Sprite> possiblePortraits;
    public List<DialogueData> possibleDialogues;

    public override Identity GetIdentity()
    {
        Identity identity = new Identity();
        identity.Name = possibleNames.GetRandomElement();
        identity.Description = possibleDescriptions.GetRandomElement();
        identity.Portrait = possiblePortraits.GetRandomElement();
        identity.Dialogue = possibleDialogues.GetRandomElement();
        return identity;
    }
}