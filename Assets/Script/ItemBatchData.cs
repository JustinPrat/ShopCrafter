using Alchemy.Inspector;
using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBatch", menuName = "ShopCrafter/ItemBatch"), Serializable]
public class ItemBatchData : ScriptableObject, ICost, IRewardable
{
    public ItemBatch ItemBatch;

#if UNITY_EDITOR
    private int amount;
    private Item item;

    private void OnValidate()
    {
        if (amount != ItemBatch.Amount || item != ItemBatch.Item)
        {
            amount = ItemBatch.Amount;
            item = ItemBatch.Item;
            EditorApplication.delayCall += () =>
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), amount.ToString() + " " + item.ItemName);
            };
        }
    }
#endif

    public bool CanPay(ManagerRefs managerRefs)
    {
        return ItemBatch.CanPay(managerRefs);
    }

    public void ResolveCost(ManagerRefs managerRefs)
    {
        ItemBatch.ResolveCost(managerRefs);
    }

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver = null)
    {
        ItemBatch.OnGetReward(managerRefs, giver);
    }

    public ICost.UIDisplayData GetCostDisplayData()
    {
        return ItemBatch.GetCostDisplayData();
    }

    public IRewardable.UIDisplayData GetRewardDisplayData()
    {
        return ItemBatch.GetRewardDisplayData();
    }
}

[Serializable]
public class ItemBatch : ICost, IRewardable
{
    public int Amount;
    public Item Item;

    public bool CanPay(ManagerRefs managerRefs)
    {
        return managerRefs.CraftingManager.HasItem(Item, Amount);
    }

    public void ResolveCost(ManagerRefs managerRefs)
    {
        managerRefs.CraftingManager.ConsumeItem(Item, Amount);
    }

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver)
    {
        managerRefs.CraftingManager.AddItem(Item, Amount);
    }

    public ICost.UIDisplayData GetCostDisplayData()
    {
        return new ICost.UIDisplayData()
        {
            Amount = this.Amount,
            DisplayName = "Item",
            Icon = Item.ItemSprite
        };
    }

    public IRewardable.UIDisplayData GetRewardDisplayData()
    {
        return new IRewardable.UIDisplayData()
        {
            DisplayName = Item.ItemName,
            Icon = Item.ItemSprite,
            HighlightColor = Item.RarityInfos.RarityColor
        };
    }
}
