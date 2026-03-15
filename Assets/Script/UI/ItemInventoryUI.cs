using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventoryUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI typeText;

    [SerializeField]
    private Image spriteImage;

    [SerializeField]
    private Image rarityImage;

    [SerializeField]
    private TextMeshProUGUI rarityText;

    [SerializeField]
    private TextMeshProUGUI numberText;

    public void Setup(Item item, int amount)
    {
        nameText.text = item.name;
        spriteImage.sprite = item.ItemSprite;
        rarityImage.sprite = item.RarityInfos.RarityIcon;
        rarityText.text = item.RarityInfos.RarityName;
        numberText.text = amount.ToString();
        typeText.text = item.Type.ToString();
    }
}
