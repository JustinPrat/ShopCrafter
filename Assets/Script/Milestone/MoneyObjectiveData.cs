using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "MoneyObjective", menuName = "ShopCrafter/Milestone/MoneyObjective")]
public class MoneyObjectiveData : ObjectiveData
{
    public override ObjectiveBehavior GetObjectiveBehavior()
    {
        MoneyObjectiveBehavior objectiveBehavior = new MoneyObjectiveBehavior();
        objectiveBehavior.Setup(this);
        return objectiveBehavior;
    }

    public class MoneyObjectiveBehavior : ObjectiveBehavior
    {
        private MoneyObjectiveData data;
        public override ObjectiveData Data => data;

        public void Setup(MoneyObjectiveData data)
        {
            this.data = data;
        }

        public override void OnStartObjective()
        {
            data.Refs.GameEventsManager.milestoneEvents.MilestoneRequireMoney(data.ReputationMax);
            data.Refs.GameEventsManager.milestoneEvents.OnMilestoneMoneyGave += OnMilestoneMoneyGave;
        }

        private void OnMilestoneMoneyGave(int obj)
        {
            GainReputation(data.ReputationAmount);

            if (ReputationGained >= data.ReputationMax)
            {
                data.Refs.GameEventsManager.milestoneEvents.MilestoneMoneyRequireStop();
            }
        }

        public override void OnStopObjective()
        {
            data.Refs.GameEventsManager.milestoneEvents.MilestoneMoneyRequireStop();
            data.Refs.GameEventsManager.milestoneEvents.OnMilestoneMoneyGave -= OnMilestoneMoneyGave;
        }
    }
}
