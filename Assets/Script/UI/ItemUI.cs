using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private CanvasGroup canvasGroup;

    private Item item;
    private CraftingView craftingView;

    bool isActivated;

    public void Setup (Item item, CraftingView craftingView)
    {
        itemImage.sprite = item.ItemSprite;
        this.item = item;
        this.craftingView = craftingView;
    }

    public void OnButtonLeftClick ()
    {
        if (isActivated) { return; }

        canvasGroup.alpha = 0.5f;
        craftingView.OnItemClick(item);
        isActivated = true;
    }

    public void OnButtonRightClick()
    {
        if (!isActivated) { return; }

        canvasGroup.alpha = 1f;
        craftingView.OnItemRemove(item);
        isActivated = false;
    }
}
