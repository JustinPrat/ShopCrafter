using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagDivideOtherScore", menuName = "ShopCrafter/Tags/TagDivideOtherScore")]
public class TagModifyOtherScore : TagEffect
{
    public Other otherPos;

    [SerializeReference]
    public StatModifier statModifier;

    public enum Other
    {
        Before = 0,
        After = 1,
        Both = 2,
        All = 3
    }

    public override int ApplyTagEffect(int score)
    {
        return score;
    }

    public override void PreSelectionApply(List<TagValue> otherTagValues, int ownIndex)
    {
        for (int i = 0; i < otherTagValues.Count; i++)
        {
            TagValue tagValue = otherTagValues[i];

            switch (otherPos)
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
            }
        }
    }

    public override void PreSelectionRemove(List<TagValue> otherTagValues, int ownIndex)
    {
        for (int i = 0; i < otherTagValues.Count; i++)
        {
            TagValue tagValue = otherTagValues[i];

            switch (otherPos)
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
            }
        }
    }
}
