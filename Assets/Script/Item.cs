using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ShopCrafter/Item")]
public class Item : ScriptableObject
{
    public EItemType Type;
    public Rarity RarityInfos;
    public Sprite ItemSprite;
}
