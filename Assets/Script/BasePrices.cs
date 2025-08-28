using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BasePrices", menuName = "ShopCrafter/BasePrices")]
public class BasePrices : ScriptableObject
{
    public PricePerRarity[] PricePerRarities = new PricePerRarity[0];

#if UNITY_EDITOR
    private void OnValidate()
    {
        int maxRarity = (int)Enum.GetValues(typeof(ERarity)).Cast<ERarity>().Last();
        if (PricePerRarities.Length < maxRarity || PricePerRarities.Length > maxRarity)
        {
            Array.Resize(ref PricePerRarities, maxRarity);
        }
    }
#endif
}

[Serializable]
public struct PricePerRarity
{
    public int MinPrice;
    public int MaxPrice;
}
