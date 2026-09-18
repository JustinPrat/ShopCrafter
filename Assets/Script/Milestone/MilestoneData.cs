using System;
using System.Collections.Generic;
using UnityEngine;
using MackySoft.Choice;
using TriInspector;

[CreateAssetMenu(fileName = "MilestoneData", menuName = "ShopCrafter/Milestone/MilestoneData")]
public class MilestoneData : ScriptableObject
{
    public bool IsSpecialCharacter;

    [HideIf(nameof(IsSpecialCharacter))]
    public int ReputationRequired;

    public int NumberTickets = 2;

    public List<Objective> Objectives;
    public List<DailyTicketWeighted> DailyTicketDatasPool;

    [Serializable]
    public struct DailyTicketWeighted
    {
        public DailyTicketData DailyTicketData;

        [Slider(0, 100)]
        public int WeightPercentChance;

        [Slider(0, 100)]
        public int WeightAfterPick;
    }

    public DailyTicketData PoolDailyTicket()
    {
        IWeightedSelector<DailyTicketWeighted> weightedSelector = DailyTicketDatasPool.ToWeightedSelector(item => item.WeightPercentChance);
        return weightedSelector.SelectItemWithUnityRandom().DailyTicketData;
    }

    public ManagerRefs Refs;

    public void OnEnterMilestone()
    {
        foreach (Objective objective in Objectives)
        {
            objective.Subscribe(Refs);
        }
    }

    public void OnExitMilestone()
    {
        foreach (Objective objective in Objectives)
        {
            objective.Unsubscribe(Refs);
        }
    }
}