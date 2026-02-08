using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ShopCrafter/Item"), Serializable]
public class Item : ScriptableObject, IRewardable
{
    public EItemType Type;
    public Rarity RarityInfos;
    public Sprite ItemSprite;

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver)
    {
        managerRefs.CraftingManager.AddItem(this);
    }
}
