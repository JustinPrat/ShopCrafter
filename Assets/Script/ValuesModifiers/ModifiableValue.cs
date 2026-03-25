using System;
using System.Collections.Generic;

[Serializable]
public class ModifiableValue
{
    public float BaseValue;

    [NonSerialized]
    public List<StatModifier> StatModifiers = new List<StatModifier>();

    public int Value { get { return (int)CalculateFinalValue(); } }

    public void AddModifier(StatModifier mod)
    {
        StatModifiers.Add(mod);
    }

    public bool RemoveModifier(StatModifier mod)
    {
        return StatModifiers.Remove(mod);
    }

    private float CalculateFinalValue()
    {
        float finalValue = BaseValue;

        for (int i = 0; i < StatModifiers.Count; i++)
        {
            finalValue = StatModifiers[i].ModifyValue(finalValue);
        }

        return (float)Math.Round(finalValue, 4);
    }
}
