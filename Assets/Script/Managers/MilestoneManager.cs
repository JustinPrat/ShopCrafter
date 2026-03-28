using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Milestone
{
    public int ReputationRequiredForSpecial;
}

public enum MilestoneState
{
    GainingReputation = 0,
    SpecialCharacter = 1
}

public class MilestoneManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private List<Milestone> milestoneList;

    private Milestone currentMilestone;
    private int currentMilestoneIndex;
    private int currentMilestoneReputation;
    private MilestoneState currentMilestoneState = MilestoneState.GainingReputation;

    [ShowInInspector]
    public Milestone CurrentMilestone => currentMilestone;
    [ShowInInspector]
    public int CurrentMilestoneIndex => currentMilestoneIndex;
    [ShowInInspector]
    public int CurrentMilestoneReputation => currentMilestoneReputation;
    [ShowInInspector]
    public MilestoneState CurrentMilestoneState => currentMilestoneState;

    private void Awake()
    {
        managerRefs.MilestoneManager = this;
        currentMilestoneIndex = 0;
        currentMilestoneReputation = 0;
        currentMilestone = milestoneList[currentMilestoneIndex];
    }

    private void Start()
    {
        managerRefs.GameEventsManager.milestoneEvents.OnGainReputation += OnGainReputation;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneUpgrade += OnMilestoneUpgrade;
    }

    private void OnMilestoneUpgrade()
    {
        currentMilestoneIndex++;
        currentMilestone = milestoneList[currentMilestoneIndex];
        currentMilestoneState = MilestoneState.GainingReputation;
        currentMilestoneReputation = 0;
        managerRefs.GameEventsManager.milestoneEvents.ReachMilestone(currentMilestoneIndex);
    }

    private void OnDestroy()
    {
        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.milestoneEvents.OnGainReputation -= OnGainReputation;
            managerRefs.GameEventsManager.milestoneEvents.OnMilestoneUpgrade -= OnMilestoneUpgrade;
        }
    }

    private void OnGainReputation(int amount)
    {
        if (currentMilestoneState != MilestoneState.GainingReputation)
            return;

        currentMilestoneReputation += amount;
        if (currentMilestoneReputation >= currentMilestone.ReputationRequiredForSpecial)
        {
            currentMilestoneReputation = 0;
            currentMilestoneState = MilestoneState.SpecialCharacter;
            managerRefs.GameEventsManager.milestoneEvents.MilestoneStateChanged(MilestoneState.SpecialCharacter);
        }
    }

#if UNITY_EDITOR
    [Button]
    private void AddReputation(int amount)
    {
        managerRefs.GameEventsManager.milestoneEvents.GainReputation(amount);
    }
#endif
}