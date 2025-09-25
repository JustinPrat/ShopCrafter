using UnityEngine;

public enum EItemType
{
    Screw,
    Gear,
    Wheels,
    Battery,
    Plate
}

[CreateAssetMenu(fileName = "ItemType", menuName = "ShopCrafter/ItemType")]
public class ItemType : ScriptableObject
{
    public EItemType Type;
    public Sprite Icon;
}
