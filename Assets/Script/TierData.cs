using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TierData", menuName = "ShopCrafter/TierData")]
public class TierData : ScriptableObject
{
    public float TierSpeed;
    public float TierTargetSize;
}

[CreateAssetMenu(fileName = "TierList", menuName = "ShopCrafter/TierList")]
public class TierList : ScriptableObject
{
    public List<TierData> Tiers;
}
