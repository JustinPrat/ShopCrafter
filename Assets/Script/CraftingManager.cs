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
}
