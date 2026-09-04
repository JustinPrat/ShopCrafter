using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TierList", menuName = "ShopCrafter/TierList")]
public class TierList : ScriptableObject
{
    public List<TierData> Tiers;
}