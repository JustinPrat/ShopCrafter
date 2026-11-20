public class IdentityFactory : Factory<Identity, IdentityData>
{
    public IdentityFactory(IdentityData data) : base(data)
    {
    }

    protected override Identity Pick()
    {
        return Data.GetIdentity();
    }
}