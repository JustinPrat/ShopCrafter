using UnityEngine;

public abstract class BaseTicketUI : MonoBehaviour
{
    public abstract void Setup(DailyTicketData.DailyTicketBehavior dailyTicketBehavior);
}
