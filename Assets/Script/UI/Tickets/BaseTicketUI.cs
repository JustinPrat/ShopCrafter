using UnityEngine;

public abstract class BaseTicketUI : MonoBehaviour
{
    public abstract bool CanClick { get; }
    public abstract void Setup(DailyTicketData.DailyTicketBehavior dailyTicketBehavior);
}
