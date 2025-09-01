using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private TextMeshProUGUI itemPrice;

    [SerializeField]
    private TextMeshProUGUI itemNumber;

    private SellingItem currentSellingItem;

    public void Setup (SellingItem sellingItem)
    {
        currentSellingItem = sellingItem;
        itemName.text = currentSellingItem.item.name;
        itemImage.sprite = currentSellingItem.item.ItemSprite;
        itemPrice.text = currentSellingItem.priceEach.ToString();
        itemNumber.text = "X" + currentSellingItem.amount.ToString();
    }
}
