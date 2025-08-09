using System.Collections.Generic;
using UnityEngine;

public enum ECraftedType
{
    Utility,
    Weapon,
    Armor,
    Consumable
}

[CreateAssetMenu(fileName = "CraftedObjectRecipe", menuName = "ShopCrafter/CraftedObjectRecipe")]
public class CraftedObjectRecipe : ScriptableObject
{
    public List<EItemType> RequiredItems;
    public ECraftedType CraftedType;
    public string CraftedName;
    public string CraftedDescription;
    public Sprite CraftedSprite;
}
