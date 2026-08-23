using System.Collections.Generic;
using UnityEngine;
using static TagModifyOtherScore;

[CreateAssetMenu(fileName = "TagModifyOtherModifiers", menuName = "ShopCrafter/Tags/TagModifyOtherModifiers")]
public class TagModifyOtherModifiers : TagEffect
{
    public Other OtherPos;

    private List<StatModifier> registeredStatModifiers;

    public override int ApplyTagEffect(int score)
    {
        return score;
    }

    public override void PreSelectionApply(List<TagValue> otherTagValues, int ownIndex)
    {
        List<StatModifier> statModifiers = otherTagValues[ownIndex].Amount.StatModifiers;
        registeredStatModifiers = new List<StatModifier>(statModifiers);

        for (int i = 0; i < otherTagValues.Count; i++)
        {
            TagValue tagValue = otherTagValues[i];
            switch (OtherPos)
            {
                case Other.Before:
                    if (i == ownIndex - 1)
                    {
                        foreach (StatModifier statModifier in registeredStatModifiers)
                        {
                            tagValue.Amount.AddModifier(statModifier);
                        }
                    }
                    break;

                case Other.After:
                    if (i == ownIndex + 1)
                    {
                        foreach (StatModifier statModifier in registeredStatModifiers)
                        {
                            tagValue.Amount.AddModifier(statModifier);
                        }
                    }
                    break;
                case Other.Both:
                    break;
                case Other.All:
                    break;
                case Other.Opposite:
                    break;
                case Other.Self:
                    break;
                default:
                    break;
            }
        }
    }

    public override void PreSelectionRemove(List<TagValue> otherTagValues, int ownIndex)
    {
        for (int i = 0; i < otherTagValues.Count; i++)
        {
            TagValue tagValue = otherTagValues[i];
            switch (OtherPos)
            {
                case Other.Before:
                    if (i == ownIndex - 1)
                    {
                        foreach (StatModifier statModifier in registeredStatModifiers)
                        {
                            tagValue.Amount.RemoveModifier(statModifier);
                        }
                    }
                    break;

                case Other.After:
                    if (i == ownIndex + 1)
                    {
                        foreach (StatModifier statModifier in registeredStatModifiers)
                        {
                            tagValue.Amount.RemoveModifier(statModifier);
                        }
                    }
                    break;
                case Other.Both:
                    break;
                case Other.All:
                    break;
                case Other.Opposite:
                    break;
                case Other.Self:
                    break;
                default:
                    break;
            }
        }
    }
}
