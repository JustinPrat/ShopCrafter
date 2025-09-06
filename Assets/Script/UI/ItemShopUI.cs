using System;
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

    [SerializeField]
    private Button buttonBuy;

    public Action<SellingItem> OnItemBuy;

    private SellingItem currentSellingItem;

    public void Setup (SellingItem sellingItem)
    {
        currentSellingItem = sellingItem;
        itemName.text = currentSellingItem.item.name;
        itemImage.sprite = currentSellingItem.item.ItemSprite;
        itemPrice.text = currentSellingItem.priceEach.ToString();
        itemNumber.text = "X" + currentSellingItem.amount.ToString();

        buttonBuy.onClick.AddListener(OnItemClick);
    }

    private void OnItemClick ()
    {
        Debug.Log("OnClickBuy : " + currentSellingItem.item.name);

        currentSellingItem.amount -= 1;
        itemNumber.text = "X" + currentSellingItem.amount.ToString();

        if (currentSellingItem.amount <= 0)
        {
            buttonBuy.interactable = false;
        }

        OnItemBuy?.Invoke(currentSellingItem);
    }
}
