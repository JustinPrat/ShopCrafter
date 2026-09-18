using TMPro;
using UnityEngine;

public class TextTicketUI : BaseTicketUI
{
    [SerializeField]
    private TextMeshProUGUI textMessage;

    public override void Setup(DailyTicketData.DailyTicketBehavior dailyTicketBehavior)
    {
        TextTicketData.TextTicketBehavior textTicketBehavior = (TextTicketData.TextTicketBehavior)dailyTicketBehavior;
        textMessage.text = textTicketBehavior.CurrentText;
    }
}
