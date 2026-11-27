using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/PNJ identity archetype")]
public class PNJIdentityArchetype : IdentityData
{
    public List<string> possibleNames;
    public List<string> possibleDescriptions;
    public List<SpriteController> possibleVisuals;
    [InlineEditor]
    public List<DialogueData> possibleDialogues;

    public override IdentityFactory GetFactory()
    {
        return new IdentityArchetypeFactory(this);
    }

    public override Identity GetIdentity()
    {
        Identity identity = new Identity();
        identity.Name = possibleNames.GetRandomElement();
        identity.Description = possibleDescriptions.GetRandomElement();

        SpriteController spriteController = possibleVisuals.GetRandomElement();
        identity.Portrait = spriteController.Sprite;
        identity.Controller = spriteController.Controller;

        identity.Dialogue = possibleDialogues.GetRandomElement();

        return identity;
    }
}

[Serializable]
public struct SpriteController
{
    public Sprite Sprite;
    public RuntimeAnimatorController Controller;
}