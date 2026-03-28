using UnityEngine;

public class MilestoneUpgradeData : IRewardable
{
    public void OnGetReward(ManagerRefs managerRefs, GameObject giver = null)
    {
        managerRefs.GameEventsManager.milestoneEvents.MilestoneUpgrade();
    }
}
