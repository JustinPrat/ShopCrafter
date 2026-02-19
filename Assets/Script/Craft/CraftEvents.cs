using System;

public class CraftEvents
{
    public event Action<CraftedObjectData> OnCraftedItem;
    public void CraftItem(CraftedObjectData craftedObject)
    {
        if (OnCraftedItem != null)
        {
            OnCraftedItem(craftedObject);
        }
    }
}
