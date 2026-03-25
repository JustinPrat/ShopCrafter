using System;

[Serializable]
public abstract class StatModifier
{
    public float Value;
    public StatModifier()
    {
        Value = 0f;
    }

    public StatModifier(float value)
    {
        Value = value;
    }

    public abstract float ModifyValue(float value);
}
