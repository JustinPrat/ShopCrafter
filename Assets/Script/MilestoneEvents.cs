using System;

public class MilestoneEvents
{
    public event Action OnMilestoneUpgrade;
    public void MilestoneUpgrade()
    {
        if (OnMilestoneUpgrade != null)
        {
            OnMilestoneUpgrade();
        }
    }

    public event Action<int> OnMilestoneReached;
    public void ReachMilestone(int newMilestoneIndex)
    {
        if (OnMilestoneReached != null)
        {
            OnMilestoneReached(newMilestoneIndex);
        }
    }

    public event Action<MilestoneState> OnMilestoneStateChanged;
    public void MilestoneStateChanged(MilestoneState newMilestoneState)
    {
        if (OnMilestoneStateChanged != null)
        {
            OnMilestoneStateChanged(newMilestoneState);
        }
    }

    public event Action<int> OnGainReputation;
    public void GainReputation(int amount)
    {
        if (OnGainReputation != null)
        {
            OnGainReputation(amount);
        }
    }
}
