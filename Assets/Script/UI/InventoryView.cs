using System.Collections.Generic;
using UnityEngine;

public class InventoryView : UIView
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject inventorySlotPrefab;

    [SerializeField]
    private Transform inventoryParent;

    private bool isInitialized;
    private int lastIndex;
    private List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();

    private void Start()
    {
        managerRefs.GameEventsManager.playerEvents.OnUpdateCraftedInventory += OnUpdateCraftedInventory;
        managerRefs.GameEventsManager.playerEvents.OnSelectedInventoryIndexChange += OnSelectedInventoryIndexChange;
    }

    private void OnDestroy()
    {
        managerRefs.GameEventsManager.playerEvents.OnUpdateCraftedInventory -= OnUpdateCraftedInventory;
        managerRefs.GameEventsManager.playerEvents.OnSelectedInventoryIndexChange -= OnSelectedInventoryIndexChange;
    }

    public void Setup(int index, Dictionary<int, CraftedObjectData> craftedInventory)
    {
        OnUpdateCraftedInventory(craftedInventory);
        OnSelectedInventoryIndexChange(index);
    }

    private void OnSelectedInventoryIndexChange(int index)
    {
        if (!isInitialized)
            return;

        inventorySlots[lastIndex].SetSelected(false);
        inventorySlots[index].SetSelected(true);

        lastIndex = index;
    }

    private void OnUpdateCraftedInventory(Dictionary<int, CraftedObjectData> craftedInventory)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            for (int i = 0; i < craftedInventory.Count; i++)
            {
                InventorySlotUI inventorySlot = Instantiate(inventorySlotPrefab, inventoryParent).GetComponent<InventorySlotUI>();
                inventorySlot.Setup(craftedInventory[i]);
                inventorySlots.Add(inventorySlot);

                inventorySlot.Button.OnLeftClick.AddListener(OnInventorySlotClick);
            }
        }
        else
        {
            for (int i = 0; i < craftedInventory.Count; i++)
            {
                inventorySlots[i].Setup(craftedInventory[i]);
            }
        }
    }

    private void OnInventorySlotClick(AdvancedButton button)
    {
        int index = inventorySlots.FindIndex(slot => slot == button.GetComponent<InventorySlotUI>());
        if (index >= 0)
        {
            managerRefs.GameEventsManager.playerEvents.SelectedInventoryIndexClick(index);
        }
    }
}
