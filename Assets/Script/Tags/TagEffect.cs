using System.Collections.Generic;
using UnityEngine;

public abstract class TagEffect : ScriptableObject
{
    public abstract void PreSelectionApply(List<TagValue> otherTagValues, int ownIndex);
    public abstract void PreSelectionRemove(List<TagValue> otherTagValues, int ownIndex);

    public abstract int ApplyTagEffect(int score);
}
