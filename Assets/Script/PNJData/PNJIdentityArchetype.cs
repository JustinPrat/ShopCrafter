using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/PNJ identity archetype")]
public class PNJIdentityArchetype : IdentityData
{
    public List<string> possibleNames;
    public List<string> possibleDescriptions;
    public List<Sprite> possiblePortraits;
    public List<DialogueData> possibleDialogues;

    private List<DialogueData> availableDialogues;

    public override Identity GetIdentity()
    {
        if (availableDialogues.Count == 0)
        {
            availableDialogues = new List<DialogueData>(possibleDialogues);
        }

        Identity identity = new Identity();
        identity.Name = possibleNames.GetRandomElement();
        identity.Description = possibleDescriptions.GetRandomElement();
        identity.Portrait = possiblePortraits.GetRandomElement();

        identity.Dialogue = availableDialogues.GetRandomElement();
        availableDialogues.Remove(identity.Dialogue);

        return identity;
    }
}