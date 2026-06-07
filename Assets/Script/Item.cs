using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ShopCrafter/Item"), Serializable]
public class Item : ScriptableObject, IRewardable, ICost
{
    [OnValueChanged(nameof(UpdateItemData))]
    public string ItemName;
    public EItemType Type;
    public Rarity RarityInfos;
    public Sprite ItemSprite;
    public List<TagValue> Tags;

#if UNITY_EDITOR
    private void UpdateItemData()
    {
        EditorApplication.delayCall += () =>
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), "Item " + ItemName);
        };
    }
#endif

    public bool CanPay(ManagerRefs managerRefs)
    {
        return managerRefs.CraftingManager.HasItem(this);
    }

    public void ResolveCost(ManagerRefs managerRefs)
    {
        managerRefs.CraftingManager.ConsumeItem(this);
    }

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver)
    {
        managerRefs.CraftingManager.AddItem(this);
    }

    public ICost.UIDisplayData GetCostDisplayData()
    {
        return new ICost.UIDisplayData()
        {
            Amount = 1,
            DisplayName = "Item",
            Icon = ItemSprite
        };
    }

    public IRewardable.UIDisplayData GetRewardDisplayData()
    {
        return new IRewardable.UIDisplayData()
        {
            DisplayName = ItemName,
            Icon = ItemSprite,
            HighlightColor = RarityInfos.RarityColor
        };
    }
}
