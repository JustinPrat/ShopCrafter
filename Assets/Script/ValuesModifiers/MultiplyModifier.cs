using System;
[Serializable]
public class MultiplyModifier : StatModifier
{
    public MultiplyModifier() : base()
    {
    }

    public override StatModifier Clone(StatModifier modifier)
    {
        return new MultiplyModifier() { Value = modifier.Value };
    }

    public override float ModifyValue(float value)
    {
        return value * Value;
    }
}
