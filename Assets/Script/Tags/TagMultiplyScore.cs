using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagMultiplyScore", menuName = "ShopCrafter/Tags/TagMultiplyScore")]
public class TagMultiplyScore : TagEffect
{
    public float MultiplyAmount = 1f;

    public override int ApplyTagEffect(int score)
    {
        return (int)Mathf.Round(score * MultiplyAmount);
    }

    public override void PreSelectionApply(List<TagValue> otherTagValues, int ownIndex)
    {
    }

    public override void PreSelectionRemove(List<TagValue> otherTagValues, int ownIndex)
    {
    }
}
