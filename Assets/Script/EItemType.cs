using UnityEngine;

public enum EItemType
{
    Screw,
    Gear,
    Wheels,
    Battery,
    Plate,
    Ancient
}

[CreateAssetMenu(fileName = "ItemType", menuName = "ShopCrafter/ItemType")]
public class ItemType : ScriptableObject
{
    public EItemType Type;
    public Sprite Icon;
}
