using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/PNJ single identity")]
public class PNJSingleIdentity : IdentityData
{
    public Identity identity;

    public override IdentityFactory GetFactory()
    {
        return new IdentityFactory(this);
    }

    public override Identity GetIdentity()
    {
        return identity;
    }
}