using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftedStatView : UIView
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Image rarityIconImage;

    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI descriptionText;

    [SerializeField]
    private TextMeshProUGUI priceText;

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);
    }

    public void Setup (CraftedObjectData craftedObjectData, Vector3 pos)
    {
        itemImage.sprite = craftedObjectData.CraftedObjectRecipe.CraftedSprite;
        rarityIconImage.sprite = craftedObjectData.Rarity.RarityIcon;
        nameText.text = craftedObjectData.CraftedObjectRecipe.CraftedName;
        descriptionText.text = craftedObjectData.CraftedObjectRecipe.CraftedDescription;
        priceText.text = craftedObjectData.GetPrice().ToString();

        transform.position = pos;
    }
}
