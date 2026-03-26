using System;

[Serializable]
public abstract class StatModifier
{
    public float Value;
    public StatModifier()
    {
        Value = 0f;
    }

    public abstract StatModifier Clone(StatModifier modifier);

    public abstract float ModifyValue(float value);
}
