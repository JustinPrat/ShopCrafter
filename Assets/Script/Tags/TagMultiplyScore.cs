using UnityEngine;

[CreateAssetMenu(fileName = "TagMultiplyScore", menuName = "ShopCrafter/Tags/TagMultiplyScore")]
public class TagMultiplyScore : TagEffect
{
    public float MultiplyAmount = 1f;

    public override int ApplyTagEffect(int score)
    {
        return (int)Mathf.Round(score * MultiplyAmount);
    }
}
