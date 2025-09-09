using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : UIView
{
    [SerializeField]
    private RectTransform itemContainer;

    [SerializeField]
    private Image portrait;

    [SerializeField]
    private ItemShopUI itemUIPrefab;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private TextMeshProUGUI coinAmountText;

    private List<ItemShopUI> itemSellingInstantiated = new List<ItemShopUI>();

    public void Setup (List<SellingItem> sellingItems, PNJBehaviour pnjBehaviour)
    {
        for (int i = itemContainer.childCount -1; i >= 0; i--)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }

        itemSellingInstantiated.Clear();
        foreach (SellingItem itemSelling in sellingItems)
        {
            ItemShopUI itemShopUI = Instantiate(itemUIPrefab, itemContainer);
            itemShopUI.Setup(itemSelling);
            itemSellingInstantiated.Add(itemShopUI);

            itemShopUI.OnItemBuy += OnItemBuy;
            itemShopUI.OnItemBuy += pnjBehaviour.OnItemBuy;
        }

        portrait.sprite = pnjBehaviour.PNJData.Portrait;
        coinAmountText.text = managerRefs.SellManager.CoinAmount.ToString();
    }

    private void OnItemBuy (SellingItem clickedItem, ItemShopUI itemShopUI)
    {
        if (managerRefs.SellManager.TryPayForItem(clickedItem.priceEach))
        {
            managerRefs.CraftingManager.AddItem(clickedItem.item);
            itemShopUI.RemoveItemBought();
            coinAmountText.text = managerRefs.SellManager.CoinAmount.ToString();
        }

        foreach (ItemShopUI itemUI in itemSellingInstantiated)
        {
            itemUI.UpdateCoinAmount();
        }
    }

    public void CloseShop () // inspector button click
    {
        managerRefs.UIManager.ToggleShopView(false);
        managerRefs.UIManager.DialogueView.NextLine();
    }
}
