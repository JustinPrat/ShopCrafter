using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextTicket", menuName = "ShopCrafter/Milestone/TextTicket")]
public class TextTicketData : DailyTicketData
{
    public List<string> TextPool;

    public override DailyTicketBehavior GetDailyTicketBehavior()
    {
        string randomText = TextPool[UnityEngine.Random.Range(0, TextPool.Count)];
        return new TextTicketBehavior(this, randomText);
    }

    public override DailyTicketPool CreatePool(float weight)
    {
        return new DailyTicketPool<string>(this, weight, MaxQuantity, TextPool, pickedText => new TextTicketBehavior(this, pickedText));
    }

    public class TextTicketBehavior : DailyTicketBehavior
    {
        private TextTicketData data;
        private string currentText;

        public string CurrentText => currentText;

        public override DailyTicketData Data => data;

        public TextTicketBehavior(TextTicketData data, string currentText)
        {
            this.data = data;
            this.currentText = currentText;
        }
    }
}
