using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingView : UIView
{
    [SerializeField]
    private Transform itemHolder;

    [SerializeField]
    private ItemUI itemUIPrefab;

    [SerializeField]
    private List<ItemUI> itemsConfirmedUI;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Sprite normalSprite;

    private List<Item> selectedItems = new List<Item>();
    private List<ItemUI> selectionSlotsUI = new List<ItemUI>();
    private Canvas canvas;

    public bool CanAdd => selectedItems.Count < 3;

    public CraftingTable CurrentCraftingTable { get; set; }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        managerRefs.InputManager.Actions.UI.Cancel.performed += OnCancelPerformed;
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.UI.Cancel.performed -= OnCancelPerformed;
    }

    private void OnCancelPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        managerRefs.UIManager.ToggleCraftingView(false, CurrentCraftingTable);
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            foreach (KeyValuePair<Item, int> item in managerRefs.CraftingManager.ItemInventory)
            {
                ItemUI itemScript = Instantiate(itemUIPrefab).GetComponent<ItemUI>();
                itemScript.Setup(item.Key, this, item.Value);
                itemScript.transform.SetParent(itemHolder, false);
                selectionSlotsUI.Add(itemScript);
            }

            if (selectionSlotsUI.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(selectionSlotsUI[0].gameObject);
            }
        }
        else
        {
            RemoveItems();
        }
    }

    private void RemoveItems ()
    {
        for (int i = selectedItems.Count - 1; i >= 0; i--)
        {
            selectedItems.RemoveAt(i);
        }

        selectedItems.Clear();

        for (int i = selectionSlotsUI.Count - 1; i >= 0; i--)
        {
            Destroy(selectionSlotsUI[i].gameObject);
        }

        selectionSlotsUI.Clear();

        for (int i = 0; i < itemsConfirmedUI.Count; i++)
        {
            if (!itemsConfirmedUI[i].IsEmpty)
            {
                itemsConfirmedUI[i].RemoveItem();
                itemsConfirmedUI[i].ItemImage.sprite = normalSprite;
            }
        }
    }

    public void ValidateCrafting ()
    {
        managerRefs.CraftingManager.ConsumeItems(selectedItems);
        managerRefs.UIManager.ToggleCraftingView(false, CurrentCraftingTable);
        managerRefs.UIManager.ToggleMiniGameView(true, CurrentCraftingTable, transform.position);
    }

    public void OnItemClick (Item item, ItemUI itemUi)
    {
        selectedItems.Add(item);
        itemUi.UpdateAmount(-1);

        foreach (ItemUI confirmedItemUI in itemsConfirmedUI)
        {
            if (confirmedItemUI.IsEmpty)
            {
                confirmedItemUI.Setup(item, this);
                break;
            }
        }
    }

    public void UpdateItemPoses(Item itemRemoved)
    {
        for (int i = 0; i < selectionSlotsUI.Count; i++)
        {
            ItemUI itemUi = selectionSlotsUI[i];
            if (itemUi.HeldItem == itemRemoved)
            {
                itemUi.UpdateAmount(1);
            }
        }

        for (int i = 0; i < itemsConfirmedUI.Count; i++)
        {
            if (i >= selectedItems.Count)
            {
                if (!itemsConfirmedUI[i].IsEmpty)
                {
                    itemsConfirmedUI[i].RemoveItem();
                    itemsConfirmedUI[i].ItemImage.sprite = normalSprite;
                }
            }
            else
            {
                itemsConfirmedUI[i].Setup(selectedItems[i], this);
            }
        }
    }
 
    public bool OnItemRemove (Item item)
    {
        if (selectedItems.Contains(item))
        {
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                if (selectedItems[i] == item)
                {
                    selectedItems.RemoveAt(i);
                    UpdateItemPoses(item);
                    return true;
                }
            }
        }

        return false;
    }
}
