using System;
using System.Collections.Generic;

public class PlayerEvents
{
    public event Action<Dictionary<int, CraftedObjectData>> OnUpdateCraftedInventory;
    public void UpdateInventory(Dictionary<int, CraftedObjectData> inventory)
    {
        if (inventory == null)
            return;

        OnUpdateCraftedInventory?.Invoke(inventory);
    }

    public event Action<int> OnSelectedInventoryIndexChange;
    public void SelectedInventoryIndexChange(int index)
    {
        OnSelectedInventoryIndexChange?.Invoke(index);
    }

    public event Action<int> OnSelectedInventoryIndexClick;
    public void SelectedInventoryIndexClick(int index)
    {
        OnSelectedInventoryIndexClick?.Invoke(index);
    }
}
