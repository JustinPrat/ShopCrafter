using System;
using UnityEngine;

public abstract class DailyTicketData : ScriptableObject
{
    public BaseTicketUI UIPrefab;
    public ManagerRefs Refs;

    public abstract DailyTicketBehavior GetDailyTicketBehavior();

    [Serializable]
    public abstract class DailyTicketBehavior
    {
        public abstract DailyTicketData Data { get; }
        public virtual void OnStartTicket() { }
        public virtual void OnStopTicket() { }
        public virtual void OnTicketClicked() { }
    }
}
