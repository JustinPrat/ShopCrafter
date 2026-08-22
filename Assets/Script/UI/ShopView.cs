using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private TextMeshProUGUI coinAmountText;

    private List<ItemShopUI> itemSellingInstantiated = new List<ItemShopUI>();

    private void Awake()
    {
        managerRefs.InputManager.Actions.UI.Cancel.started += OnCancel;
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.UI.Cancel.started -= OnCancel;
    }

    public void Setup (List<SellingItem> sellingItems, SellerRuntime sellerTrait, PNJBrain pnjBrain)
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
            itemShopUI.OnItemBuy += ((IPNJTraitRuntime)sellerTrait).OnItemBuy;
        }

        if (managerRefs.InputManager.IsGamepad && itemSellingInstantiated.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(itemSellingInstantiated[0].BuyButton.gameObject);
        }

        portrait.sprite = pnjBrain.Data.Identity.Portrait;
        coinAmountText.text = managerRefs.SellManager.CoinAmount.ToString();
    }

    protected override void OnInputDeviceChanged()
    {
        base.OnInputDeviceChanged();

        if (managerRefs.InputManager.IsGamepad && itemSellingInstantiated.Count > 0)
        {
            StartCoroutine(SelectButtonAfterFrame(itemSellingInstantiated[0].gameObject));
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void OnCancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (gameObject.activeInHierarchy)
        {
            CloseShop();
        }
    }

    private void OnItemBuy (SellingItem clickedItem, ItemShopUI itemShopUI)
    {
        bool needToUpdateFocus = false;
        if (managerRefs.SellManager.TryPayForItem(clickedItem.priceEach))
        {
            managerRefs.CraftingManager.AddItem(clickedItem.item);
            itemShopUI.RemoveItemBought();
            coinAmountText.text = managerRefs.SellManager.CoinAmount.ToString();

            if (clickedItem.amount - 1 <= 0)
            {
                needToUpdateFocus = true;
            }
        }

        foreach (ItemShopUI itemUI in itemSellingInstantiated)
        {
            itemUI.UpdateCoinAmount();

            if (managerRefs.InputManager.IsGamepad && needToUpdateFocus && itemShopUI != itemUI && itemUI.BuyButton.enabled)
            {
                EventSystem.current.SetSelectedGameObject(itemUI.BuyButton.gameObject);
            }
        }

        if (!managerRefs.InputManager.IsGamepad)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void CloseShop () // inspector button click
    {
        managerRefs.UIManager.ToggleShopView(false);
        managerRefs.UIManager.DialogueView.NextLine();
    }
}
