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

    public Dictionary<Item, int> ItemInventory => itemInventory;

    private void Awake()
    {
        managerRefs.CraftingManager = this;
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

    //public CraftedObjectData PoolCraftedItem ()
    //{

    //}
}
