using UnityEngine;

public class MilestoneUpgradeData : IRewardable
{
    public IRewardable.UIDisplayData GetRewardDisplayData()
    {
        return new IRewardable.UIDisplayData()
        {
            DisplayName = "Milestone Upgrade",
            Icon = ResourcesConstants.MilestoneUpgradeIcon
        };
    }

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver = null)
    {
        managerRefs.GameEventsManager.milestoneEvents.MilestoneUpgrade();
    }
}
