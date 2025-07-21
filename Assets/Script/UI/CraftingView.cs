using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingView : UIView
{
    [SerializeField]
    private Transform itemHolder;

    [SerializeField]
    private ItemUI itemUI;

    [SerializeField]
    private List<Image> itemsConfirmed;

    [SerializeField]
    private ManagerRefs managerRefs;

    private List<Item> selectedItems = new List<Item>();

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            foreach (KeyValuePair<Item, int> item in managerRefs.CraftingManager.ItemInventory)
            {
                ItemUI itemScript = Instantiate(itemUI).GetComponent<ItemUI>();
                itemScript.Setup(item.Key, this);
                itemScript.transform.SetParent(itemHolder, false);
            }
        }
    }

    public void OnItemClick (Item item)
    {
        if (selectedItems.Count < 3)
        {
            itemsConfirmed[selectedItems.Count].sprite = item.ItemSprite;
            selectedItems.Add(item);
        }
    }

    public void OnItemRemove (Item item)
    {
        if (selectedItems.Contains(item))
        {
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                if (selectedItems[i] == item)
                {
                    itemsConfirmed[i].sprite = null;
                    selectedItems.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
