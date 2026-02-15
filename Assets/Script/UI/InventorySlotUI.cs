using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private Image itemBG;

    [SerializeField]
    private Sprite selectedBG;

    [SerializeField]
    private Sprite defaultBG;

    [SerializeField]
    private Sprite emptyItem;

    public void Setup (CraftedObjectData data)
    {
        if (data == null)
        {
            itemImage.sprite = emptyItem;
        }
        else
        {
            itemImage.sprite = data.CraftedObjectRecipe.CraftedSprite;
        }
    }

    public void SetSelected (bool isSelected)
    {
        if (isSelected)
        {
            itemBG.sprite = selectedBG;
        }
        else
        {
            itemBG.sprite = defaultBG;
        }
    }
}
