using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagModifyOtherScore", menuName = "ShopCrafter/Tags/TagModifyOtherScore")]
public class TagModifyOtherScore : TagEffect
{
    public bool UseOwnValue;
    public Other OtherPos;

    [SerializeReference]
    public StatModifier statModifier;

    public enum Other
    {
        Before = 0,
        After = 1,
        Both = 2,
        All = 3,
        Opposite = 4,
        Self = 5
    }

    public override int ApplyTagEffect(int score)
    {
        return score;
    }

    public override void PreSelectionApply(List<TagValue> otherTagValues, int ownIndex)
    {
        if (UseOwnValue)
            statModifier.Value = otherTagValues[ownIndex].Amount.Value;

        for (int i = 0; i < otherTagValues.Count; i++)
        {
            TagValue tagValue = otherTagValues[i];

            switch (OtherPos)
            {
                case Other.Before:
                    if (i == ownIndex - 1)
                    {
                        tagValue.Amount.AddModifier(statModifier);
                    }
                    break;

                case Other.After:
                    if (i == ownIndex + 1)
                    {
                        tagValue.Amount.AddModifier(statModifier);
                    }
                    break;

                case Other.Both:
                    if (i == ownIndex - 1)
                    {
                        tagValue.Amount.AddModifier(statModifier);
                    }
                    if (i == ownIndex + 1)
                    {
                        tagValue.Amount.AddModifier(statModifier);
                    }
                    break;

                case Other.All:
                    break;
                
                case Other.Opposite:
                    if (i == 0)
                    {
                        tagValue.Amount.AddModifier(statModifier);
                    }
                    if (i == otherTagValues.Count - 1)
                    {
                        tagValue.Amount.AddModifier(statModifier);
                    }
                    break;
                    
                case Other.Self:
                    if (i == ownIndex)
                    {
                        tagValue.Amount.AddModifier(statModifier);
                    }
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
                        tagValue.Amount.RemoveModifier(statModifier);
                    }
                    break;

                case Other.After:
                    if (i == ownIndex + 1)
                    {
                        tagValue.Amount.RemoveModifier(statModifier);
                    }
                    break;

                case Other.Both:
                    if (i == ownIndex - 1)
                    {
                        tagValue.Amount.RemoveModifier(statModifier);
                    }
                    if (i == ownIndex + 1)
                    {
                        tagValue.Amount.RemoveModifier(statModifier);
                    }
                    break;

                case Other.All:
                    break;

                case Other.Opposite:
                    if (i == 0)
                    {
                        tagValue.Amount.RemoveModifier(statModifier);
                    }
                    if (i == otherTagValues.Count - 1)
                    {
                        tagValue.Amount.RemoveModifier(statModifier);
                    }
                    break;

                case Other.Self:
                    if (i == ownIndex)
                    {
                        tagValue.Amount.RemoveModifier(statModifier);
                    }
                    break;
            }
        }
    }
}
