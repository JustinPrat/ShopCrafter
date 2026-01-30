using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/PNJ single identity")]
public class PNJSingleIdentity : IdentityData
{
    public Identity identity;

    public override Identity GetIdentity()
    {
        return identity;
    }
}