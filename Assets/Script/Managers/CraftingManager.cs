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

    private int numberItemCrafted;

    private List<CraftedObjectRecipe> craftedRecipes = new List<CraftedObjectRecipe>();
    private List<CraftedObjectRecipe> blueprintRecipes = new List<CraftedObjectRecipe>();

    public event Action<int> OnItemCraft;
    public Dictionary<Item, int> ItemInventory => itemInventory;
    public RarityHierarchy RarityHierarchy => rarityHierarchy;
    public CraftedObject CraftedObjectPrefab => craftedObjectPrefab;
    public BasePrices BasePrices => basePrices;
    public List<CraftedObjectRecipe> CraftedRecipes => craftedRecipes;
    public List<CraftedObjectRecipe> BlueprintRecipes => blueprintRecipes;
    public CraftedObjectPool CurrentCraftedObjectPool => currentCraftedObjectPool;

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

    public void ConsumeItem(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            ConsumeItem(item);
        }
    }

    public void AddItem (Item item)
    {
        if (itemInventory.ContainsKey(item))
        {
            itemInventory[item] += 1;
        }
        else
        {
            itemInventory.Add(item, 1);
        }
    }

    public void AddItem(Item item, int amount)
    {
        for (int i = 0;i < amount; i++)
        {
            AddItem(item);
        }
    }

    public bool HasItem (Item item)
    {
        return itemInventory.ContainsKey(item);
    }

    public bool HasItem(Item item, int amount)
    {
        return itemInventory.ContainsKey(item) && itemInventory[item] >= amount;
    }

    public void AddBlueprint (CraftedObjectRecipe recipeBlueprint)
    {
        if (!blueprintRecipes.Contains(recipeBlueprint))
            blueprintRecipes.Add(recipeBlueprint);
    }

    public CraftedObjectRecipe PoolCraftedItem(List<Item> items, out bool isNew, out List<TagValue> tags)
    {
        CraftedObjectRecipe recipe = currentCraftedObjectPool.FindCraftableRecipe(items, out tags);
        if (!craftedRecipes.Contains(recipe))
        {
            isNew = true;
            craftedRecipes.Add(recipe);
        }
        else
        {
            isNew = false;
        }

        return recipe;
    }

    public static List<TagValue> CombineItemTags(List<Item> items)
    {
        List<TagValue> values = new List<TagValue>();
        foreach (Item item in items)
        {
            foreach (TagValue tagValue in item.Tags)
            {
                bool hasTag = false;
                foreach (TagValue tagTemp in values)
                {
                    if (tagTemp.Asset == tagValue.Asset)
                    {
                        tagTemp.Amount.BaseValue += tagValue.Amount.BaseValue;
                        hasTag = true;
                        break;
                    }
                }

                if (!hasTag)
                {
                    values.Add(new TagValue(tagValue));
                }
            }
        }

        return values;
    }

    public CraftedObject CraftItem (CraftedObjectData craftedObjectData, int boostNumber = 0, StatModifier modifier = null)
    {
        craftedObjectData.BoostRarity(boostNumber);
        craftedObjectData.AddModifier(modifier);

        CraftedObject craftedObject = Instantiate(managerRefs.CraftingManager.CraftedObjectPrefab);
        craftedObject.Init(craftedObjectData);

        numberItemCrafted++;
        OnItemCraft?.Invoke(numberItemCrafted);
        managerRefs.GameEventsManager.milestoneEvents.GainReputation(craftedObjectData.Rarity.ReputationGain);

        return craftedObject;
    }

    public CraftedObjectData GetCraftedData(CraftedObjectRecipe recipe, bool isNew)
    {
        CraftedObjectData craftedObjectData = new CraftedObjectData(recipe, managerRefs, isNew);
        return craftedObjectData;
    }
}
