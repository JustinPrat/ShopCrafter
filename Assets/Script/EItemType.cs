using UnityEngine;

public enum EItemType
{
    Mineral,
    Organic,
    Manifactured
}

[CreateAssetMenu(fileName = "ItemType", menuName = "ShopCrafter/ItemType")]
public class ItemType : ScriptableObject
{
    public EItemType Type;
    public Sprite Icon;
}
