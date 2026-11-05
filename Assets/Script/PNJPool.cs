using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PNJPool", menuName = "ShopCrafter/PNJPool")]
public class PNJPool : ScriptableObject
{
    [InlineEditor]
    public List<PNJInfoData> PNJPoolList;

    [InlineEditor]
    public List<PNJInfoData> SpecialPNJPool;
}
