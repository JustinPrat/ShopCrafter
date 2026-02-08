using UnityEngine;

public interface IRewardable
{
    public void OnGetReward(ManagerRefs managerRefs, GameObject giver = null);
}
