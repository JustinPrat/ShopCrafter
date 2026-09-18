using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TextTicket", menuName = "ShopCrafter/Milestone/TextTicket")]
public class TextTicketData : DailyTicketData
{
    public List<string> TextPool;

    public override DailyTicketBehavior GetDailyTicketBehavior()
    {
        TextTicketBehavior textTicketBehavior = new TextTicketBehavior();
        textTicketBehavior.Setup(this);
        return textTicketBehavior;
    }

    public class TextTicketBehavior : DailyTicketBehavior
    {
        private TextTicketData data;
        private string currentText;

        public string CurrentText => currentText;

        public override DailyTicketData Data => data;

        public void Setup(TextTicketData data)
        {
            this.data = data;
        }

        public override void OnStartTicket()
        {
            base.OnStartTicket();
            currentText = data.TextPool.GetRandomElement();
        }
    }
}
