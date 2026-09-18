using TNRD;
using UnityEngine;
[CreateAssetMenu(fileName = "RewardTicket", menuName = "ShopCrafter/Milestone/RewardTicket")]
public class RewardTicketData : DailyTicketData
{
    public SerializableInterface<IRewardable> Reward;

    [TextArea]
    public string Message;

    public override DailyTicketBehavior GetDailyTicketBehavior()
    {
        RewardTicketBehavior behavior = new RewardTicketBehavior();
        behavior.Setup(this);
        return behavior;
    }

    public class RewardTicketBehavior : DailyTicketBehavior
    {
        private RewardTicketData data;
        private bool hasUsedTicket;
        public override DailyTicketData Data => data;
        public RewardTicketData RewardData => data;

        public void Setup(RewardTicketData data)
        {
            this.data = data;
        }

        public override void OnTicketClicked()
        {
            base.OnTicketClicked();

            if (!hasUsedTicket)
            {
                hasUsedTicket = true;
                data.Reward.Value.OnGetReward(data.Refs);
            }
        }
    }
}
