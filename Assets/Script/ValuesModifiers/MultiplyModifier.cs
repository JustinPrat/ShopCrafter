using System;
[Serializable]
public class MultiplyModifier : StatModifier
{
    public MultiplyModifier() : base()
    {
    }

    public MultiplyModifier(float value) : base(value)
    {
    }

    public override float ModifyValue(float value)
    {
        return value * Value;
    }
}
