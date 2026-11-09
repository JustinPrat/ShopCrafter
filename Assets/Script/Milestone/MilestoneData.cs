using System;
using System.Collections.Generic;
using UnityEngine;
using MackySoft.Choice;
using TNRD;
using Alchemy.Inspector;

[CreateAssetMenu(fileName = "MilestoneData", menuName = "ShopCrafter/Milestone/MilestoneData")]
public class MilestoneData : ScriptableObject
{
    public bool IsSpecialCharacter;

    [HideIf(nameof(IsSpecialCharacter))]
    public int ReputationRequired;

    public int NumberTickets = 2;

    [InlineEditor]
    public List<ObjectiveData> Objectives;
    public List<DailyTicketWeighted> DailyTicketDatasPool;

    public List<SerializableInterface<IRewardable>> CompleteRewards;

    [Serializable]
    public struct DailyTicketWeighted
    {
        public DailyTicketData DailyTicketData;

        [Range(0, 100)]
        public int WeightPercentChance;
    }

    public DailyTicketData PoolDailyTicket()
    {
        IWeightedSelector<DailyTicketWeighted> weightedSelector = DailyTicketDatasPool.ToWeightedSelector(item => item.WeightPercentChance);
        return weightedSelector.SelectItemWithUnityRandom().DailyTicketData;
    }

    public ManagerRefs Refs;

    public void OnEnterMilestone(List<ObjectiveBehavior> objectiveBehaviors)
    {
        objectiveBehaviors.Clear();

        foreach (ObjectiveData objective in Objectives)
        {
            ObjectiveBehavior behavior = objective.GetObjectiveBehavior();
            objectiveBehaviors.Add(behavior);
            behavior.OnStartObjective();
        }
    }

    public void OnExitMilestone(List<ObjectiveBehavior> objectiveBehaviors)
    {
        foreach (ObjectiveBehavior objective in objectiveBehaviors)
        {
            objective.OnStopObjective();
        }
    }
}