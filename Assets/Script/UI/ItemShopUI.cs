using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
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
    private AdvancedButton buttonBuy;

    [SerializeField]
    private CanvasGroup buttonCanvasGroup;

    [SerializeField]
    private ManagerRefs managerRefs;

    public Action<SellingItem, ItemShopUI> OnItemBuy;
    public AdvancedButton BuyButton => buttonBuy;
    
    private SellingItem currentSellingItem;

    public void Setup (SellingItem sellingItem)
    {
        currentSellingItem = sellingItem;
        itemName.text = currentSellingItem.item.name;
        itemImage.sprite = currentSellingItem.item.ItemSprite;
        itemPrice.text = currentSellingItem.priceEach.ToString();
        itemNumber.text = "X" + currentSellingItem.amount.ToString();

        if (managerRefs.SellManager.CoinAmount < currentSellingItem.priceEach)
        {
            DisableItemButton();
        }

        buttonBuy.OnLeftClick.AddListener(OnItemClick);
    }

    private void OnItemClick (AdvancedButton button)
    {
        OnItemBuy?.Invoke(currentSellingItem, this);
    }

    public void RemoveItemBought ()
    {
        currentSellingItem.amount -= 1;
        itemNumber.text = "X" + currentSellingItem.amount.ToString();

        if (currentSellingItem.amount <= 0)
        {
            DisableItemButton();
        }
    }

    private void DisableItemButton ()
    {
        buttonCanvasGroup.alpha = 0.5f;
        buttonBuy.enabled = false;
    }

    public void UpdateCoinAmount ()
    {
        buttonCanvasGroup.alpha = managerRefs.SellManager.CoinAmount >= currentSellingItem.priceEach && currentSellingItem.amount > 0 ? 1f : 0.5f;
    }
}
