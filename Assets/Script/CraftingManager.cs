using System;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Serialization;

[AlchemySerialize]
public partial class CraftingManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [AlchemySerializeField, NonSerialized]
    private Dictionary<Item, int> itemInventory = new Dictionary<Item, int>();

    [SerializeField]
    private CraftedObjectPool currentCraftedObjectPool;

    [SerializeField]
    private CraftedObject craftedObjectPrefab;

    [SerializeField]
    private RarityHierarchy rarityHierarchy;

    [SerializeField]
    private BasePrices basePrices;

    public Dictionary<Item, int> ItemInventory => itemInventory;
    public RarityHierarchy RarityHierarchy => rarityHierarchy;
    public CraftedObject CraftedObjectPrefab => craftedObjectPrefab;
    public BasePrices BasePrices => basePrices;

    public Action<List<Item>> OnItemsConsumed;

    private void Awake()
    {
        managerRefs.CraftingManager = this;
    }

    public void ConsumeItems (List<Item> items)
    {
        foreach (Item item in items)
        {
            ConsumeItem(item);
        }

        OnItemsConsumed?.Invoke(items);
    }

    public void ConsumeItem (Item item)
    {
        if (itemInventory.ContainsKey(item))
        {
            if (itemInventory[item] - 1 <= 0)
            {
                itemInventory.Remove(item);
            }
            else
            {
                itemInventory[item] -= 1;
            }
        }
    }

    public CraftedObjectRecipe PoolCraftedItem(List<Item> items)
    {
        return currentCraftedObjectPool.FindCraftableRecipe(items);
    }
}
