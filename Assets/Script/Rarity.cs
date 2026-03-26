using UnityEngine;

[CreateAssetMenu(fileName = "Rarity", menuName = "ShopCrafter/Rarity")]
public class Rarity : ScriptableObject
{
    public ERarity ERarity;
    public Sprite RarityIcon;
    public Color RarityColor;
    public string RarityName;
    public string RarityDescription;

    [SerializeReference]
    public StatModifier MaxStatModifier;
}

public enum ERarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Unique
}
