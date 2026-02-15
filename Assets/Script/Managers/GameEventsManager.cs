using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    #region Events

    public QuestEvents questEvents;
    public PlayerEvents playerEvents;
    public DayEvents dayEvents;

    public Action<int> OnMoneyGained;

    #endregion

    private void Awake()
    {
        managerRefs.GameEventsManager = this;
        questEvents = new QuestEvents();
        playerEvents = new PlayerEvents();
        dayEvents = new DayEvents();
    }
}
