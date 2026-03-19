using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ShopCrafter/MoneyBatch"), Serializable]
public class MoneyBatchData : ScriptableObject, ICost, IRewardable
{
    public MoneyBatch MoneyBatch;

    public bool CanPay(ManagerRefs managerRefs)
    {
        return MoneyBatch.CanPay(managerRefs);
    }

    public void ResolveCost(ManagerRefs managerRefs)
    {
        MoneyBatch.ResolveCost(managerRefs);
    }

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver = null)
    {
        MoneyBatch.OnGetReward(managerRefs, giver);
    }

    public ICost.UIDisplayData GetDisplayData()
    {
        return MoneyBatch.GetDisplayData();
    }
}

[Serializable]
public class MoneyBatch : ICost, IRewardable
{
    public int Amount;

    public bool CanPay(ManagerRefs managerRefs)
    {
        return managerRefs.SellManager.CoinAmount >= Amount;
    }

    public ICost.UIDisplayData GetDisplayData()
    {
        return new ICost.UIDisplayData()
        {
            Amount = Amount,
            DisplayName = "Coins",
            Icon = Resources.Load<Sprite>("CoinIcon")
        };
    }

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver = null)
    {
        managerRefs.SellManager.GainMoney(Amount);
    }

    public void ResolveCost(ManagerRefs managerRefs)
    {
        managerRefs.SellManager.TryPayForItem(Amount);
    }
}
