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

    [SerializeField]
    private TextMeshProUGUI itemNameText;

    [SerializeField]
    private Sequencer.Sequencer onSetItem;

    [SerializeField]
    private Sequencer.Sequencer onRemoveSetItem;

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

        if (itemNameText != null)
        {
            itemNameText.text = item.ItemName;
        }

        this.item = item;
        this.craftingView = craftingView;
        this.itemAmount = itemAmount;
        this.itemNameText.text = item.ItemName;

        if (onSetItem != null)
        {
            if (onRemoveSetItem != null)
                onRemoveSetItem.StopSequence();

            onSetItem.StartSequence();
        }
    }

    public void UpdateAmount (int toAdd)
    {
        itemAmount += toAdd;

        if (amountText != null)
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

    public void RemoveItem(bool instant = false)
    {
        item = null;
        canvasGroup.alpha = 1f;

        if (amountText != null)
        {
            amountText.text = "";
        }

        if (itemNameText != null)
        {
            itemNameText.text = "";
        }

        if (onRemoveSetItem != null)
        {
            if (onSetItem != null)
                onSetItem.StopSequence();

            if (!instant)
                onRemoveSetItem.StartSequence();
            else
                onRemoveSetItem.SkipSequence();
        }
    }

    public void OnButtonLeftClick ()
    {
        if (craftingView == null || !craftingView.CanAdd || itemAmount <= 0) 
            return;

        craftingView.OnItemClick(item, this);
    }

    public void OnButtonRightClick()
    {
        if (item == null) 
            return;

        craftingView.OnItemRemove(item);
    }
}
