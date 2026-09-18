using System;

public class MilestoneEvents
{
    public event Action OnMilestoneSpecial;
    public void MilestoneUpgrade()
    {
        if (OnMilestoneSpecial != null)
        {
            OnMilestoneSpecial();
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

    public event Action<int> OnGainReputation;
    public void GainReputation(int amount)
    {
        if (OnGainReputation != null)
        {
            OnGainReputation(amount);
        }
    }

    public event Action OnValidateMilestone;
    public void ValidateMilestone()
    {
        OnValidateMilestone?.Invoke();
    }

    public event Action OnMilestoneReady;
    public void MilestoneReady()
    {
        OnMilestoneReady?.Invoke();
    }

    public event Action OnTicketReroll;
    public void TicketReroll()
    {
        OnTicketReroll?.Invoke();
    }
}
