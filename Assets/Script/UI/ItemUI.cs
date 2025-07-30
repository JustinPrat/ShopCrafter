using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private TextMeshProUGUI amountText;

    private Item item;
    private CraftingView craftingView;

    private int itemAmount;

    public Item HeldItem => item;
    public bool IsEmpty => item == null;

    public Image ItemImage => itemImage;

    public void Setup (Item item, CraftingView craftingView, int itemAmount = 1)
    {
        itemImage.sprite = item.ItemSprite;

        if (amountText != null )
        {
            amountText.text = itemAmount.ToString();
        }

        this.item = item;
        this.craftingView = craftingView;
        this.itemAmount = itemAmount;
    }

    public void UpdateAmount (int toAdd)
    {
        itemAmount += toAdd;
        amountText.text = itemAmount.ToString();

        if (itemAmount > 0)
        {
            canvasGroup.alpha = 1f;
        }
        else
        {
            canvasGroup.alpha = 0.5f;
        }
    }

    public void RemoveItem ()
    {
        item = null;
        canvasGroup.alpha = 1f;

        if (amountText != null)
        {
            amountText.text = "";
        }
    }

    public void OnButtonLeftClick ()
    {
        if (!craftingView.CanAdd || itemAmount <= 0) { return; }

        craftingView.OnItemClick(item, this);
    }

    public void OnButtonRightClick()
    {
        craftingView.OnItemRemove(item);
    }
}
