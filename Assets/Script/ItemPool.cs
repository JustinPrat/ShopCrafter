using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPool", menuName = "ShopCrafter/ItemPool")]
public class ItemPool : ScriptableObject
{
    public List<Item> itemPool;
}