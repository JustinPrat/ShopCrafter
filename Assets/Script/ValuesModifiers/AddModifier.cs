using System;
[Serializable]
public class AddModifier : StatModifier
{
    public AddModifier() : base()
    {
    }

    public override StatModifier Clone(StatModifier modifier)
    {
        return new AddModifier() { Value = modifier.Value };
    }

    public override float ModifyValue(float value)
    {
        return value + Value;
    }
}
