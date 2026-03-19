using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ShopCrafter/Item"), Serializable]
public class Item : ScriptableObject, IRewardable, ICost
{
    public EItemType Type;
    public Rarity RarityInfos;
    public Sprite ItemSprite;

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

    public ICost.UIDisplayData GetDisplayData()
    {
        return new ICost.UIDisplayData()
        {
            Amount = 1,
            DisplayName = "Item",
            Icon = ItemSprite
        };
    }
}
