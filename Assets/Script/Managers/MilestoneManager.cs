using Alchemy.Inspector;
using MackySoft.Choice;
using System.Collections.Generic;
using UnityEngine;
using static MilestoneData;

public class MilestoneManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private List<MilestoneData> milestoneList;

    private MilestoneData currentMilestone;
    private int currentMilestoneIndex;
    private int currentMilestoneReputation;
    private bool milestoneReady;
    private List<DailyTicketData.DailyTicketBehavior> milestoneDailyTickets = new List<DailyTicketData.DailyTicketBehavior>();
    private List<DailyTicketData.DailyTicketBehavior> currentDailyTicketsBehavior = new List<DailyTicketData.DailyTicketBehavior>();

    [ShowInInspector]
    public MilestoneData CurrentMilestone => currentMilestone;
    [ShowInInspector]
    public int CurrentMilestoneIndex => currentMilestoneIndex;
    [ShowInInspector]
    public int CurrentMilestoneReputation => currentMilestoneReputation;
    [ShowInInspector]
    public bool MilestoneReady => milestoneReady;

    [ShowInInspector]
    public List<DailyTicketData.DailyTicketBehavior> MilestoneDailyTickets => milestoneDailyTickets;

    private void Awake()
    {
        managerRefs.MilestoneManager = this;
        currentMilestoneIndex = 0;
        currentMilestoneReputation = 0;
        currentMilestone = milestoneList[currentMilestoneIndex];
    }

    private void Start()
    {
        currentMilestone.OnEnterMilestone();
        managerRefs.GameEventsManager.milestoneEvents.OnGainReputation += OnGainReputation;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneSpecial += OnMilestoneSpecial;
        managerRefs.GameEventsManager.milestoneEvents.OnValidateMilestone += TryValidateMilestone;
        managerRefs.GameEventsManager.dayEvents.OnStartDay += OnStartDay;
    }
    private void OnDestroy()
    {
        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.milestoneEvents.OnGainReputation -= OnGainReputation;
            managerRefs.GameEventsManager.milestoneEvents.OnMilestoneSpecial -= OnMilestoneSpecial;
            managerRefs.GameEventsManager.milestoneEvents.OnValidateMilestone -= TryValidateMilestone;
            managerRefs.GameEventsManager.dayEvents.OnStartDay -= OnStartDay;
        }
    }

    private void TryValidateMilestone()
    {
        if (milestoneReady)
        {
            UpgradeMilestone();
        }
    }

    private void UpgradeMilestone()
    {
        milestoneReady = false;
        currentMilestoneReputation = 0;

        currentMilestoneIndex++;
        currentMilestone.OnExitMilestone();
        currentMilestone = milestoneList[currentMilestoneIndex];
        currentMilestone.OnEnterMilestone();

        RerollDailyTicket(currentMilestone);
        managerRefs.GameEventsManager.milestoneEvents.ReachMilestone(currentMilestoneIndex);
    }

    private void RerollDailyTicket(MilestoneData milestone)
    {
        foreach (DailyTicketData.DailyTicketBehavior currentTicket in milestoneDailyTickets)
        {
            currentTicket.OnStopTicket();
        }

        milestoneDailyTickets.Clear();

        List<DailyTicketWeighted> tmpList = new List<DailyTicketWeighted>(milestone.DailyTicketDatasPool);
        for (int i = 0; i < milestone.NumberTickets; i++)
        {
            if (milestone.DailyTicketDatasPool.Count <= 0)
                break;

            IWeightedSelector<DailyTicketWeighted> selector = tmpList.ToWeightedSelector(item => item.WeightPercentChance);
            DailyTicketWeighted dailyTicketWeighted = selector.SelectItemWithUnityRandom();
            dailyTicketWeighted.WeightPercentChance = dailyTicketWeighted.WeightAfterPick;

            for (int j = 0; j < tmpList.Count; j++)
            {
                if (tmpList[j].DailyTicketData == dailyTicketWeighted.DailyTicketData)
                {
                    tmpList[j] = dailyTicketWeighted;
                }
            }

            DailyTicketData.DailyTicketBehavior newTicket = dailyTicketWeighted.DailyTicketData.GetDailyTicketBehavior();
            newTicket.OnStartTicket();
            milestoneDailyTickets.Add(newTicket);
        }

        managerRefs.GameEventsManager.milestoneEvents.TicketReroll();
    }

    private void OnGainReputation(int amount)
    {
        currentMilestoneReputation += amount;

        if (currentMilestoneReputation >= currentMilestone.ReputationRequired)
        {
            milestoneReady = true;
            managerRefs.GameEventsManager.milestoneEvents.MilestoneReady();
        }
    }

    private void OnMilestoneSpecial()
    {
        milestoneReady = true;
        managerRefs.GameEventsManager.milestoneEvents.MilestoneReady();
    }

    private void OnStartDay()
    {
        RerollDailyTicket(currentMilestone);
    }

#if UNITY_EDITOR
    [Button]
    private void AddReputation(int amount)
    {
        managerRefs.GameEventsManager.milestoneEvents.GainReputation(amount);
    }
#endif
}