using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour, IInitializableManager
{
    [SerializeField]
    private ManagerRefs managerRefs;

    #region Events

    public QuestEvents questEvents;
    public PlayerEvents playerEvents;
    public DayEvents dayEvents;
    public CraftEvents craftEvents;
    public MilestoneEvents milestoneEvents;

    public Action<int> OnMoneyGained;
    public Action<int> OnMoneyUpdated;

    public Action<PNJBrain, DialogueData> OnPNJTalked;
    public Action<SpecialDialogue> OnSpecialDialogueUsed;
    public Action OnUnlockPriceVariation;

    #endregion

    public void Register(ManagerRefs refs)
    {
        managerRefs.GameEventsManager = this;

        questEvents = new QuestEvents();
        playerEvents = new PlayerEvents();
        dayEvents = new DayEvents();
        craftEvents = new CraftEvents();
        milestoneEvents = new MilestoneEvents();
    }

    public void Init(ManagerRefs refs)
    {
    }
}
