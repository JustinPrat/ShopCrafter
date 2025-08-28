using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RarityHierarchy", menuName = "ShopCrafter/RarityHierarchy")]
public class RarityHierarchy : ScriptableObject
{
    public Rarity[] RarityList;
    public Rarity UniqueRarity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        int maxRarity = (int)Enum.GetValues(typeof(ERarity)).Cast<ERarity>().Last();
        if (RarityList.Length < maxRarity || RarityList.Length > maxRarity)
        {
            Array.Resize(ref RarityList, maxRarity);
        }
    }
#endif
}
