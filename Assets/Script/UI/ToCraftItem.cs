using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToCraftItem : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private Button validateButton;

    public Button ValidateButton => validateButton;

    public void Setup(CraftedObjectRecipe itemCraft)
    {
        itemImage.sprite = itemCraft.CraftedSprite;
        itemName.text = itemCraft.CraftedName;
    }
}
