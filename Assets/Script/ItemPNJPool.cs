using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPNJPool", menuName = "ShopCrafter/ItemPNJPool")]
public class ItemPNJPool : ScriptableObject
{
    public List<ItemPoolElement> Pool;
}

public struct ItemPoolElement
{
    public int MinAmount;
    public int MaxAmount;

    public int MinPrice;
    public int MaxPrice;
}