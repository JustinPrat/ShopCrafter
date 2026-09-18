using TMPro;
using UnityEngine;

public class RewardTicketUI : BaseTicketUI
{
    [SerializeField]
    private AdvancedButton advancedButton;

    [SerializeField]
    private TextMeshProUGUI messageText;

    [SerializeField]
    private RewardElementUI rewardUI;

    private RewardTicketData.RewardTicketBehavior rewardTicketBehavior;

    public override void Setup(DailyTicketData.DailyTicketBehavior dailyTicketBehavior)
    {
        rewardTicketBehavior = (RewardTicketData.RewardTicketBehavior)dailyTicketBehavior;
        rewardUI.Setup(rewardTicketBehavior.RewardData.Reward.Value);
        messageText.text = rewardTicketBehavior.RewardData.Message;
    }

    private void Awake()
    {
        advancedButton.OnLeftClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        advancedButton.OnLeftClick.RemoveListener(OnClick);
    }

    private void OnClick(AdvancedButton button)
    {
        rewardTicketBehavior.OnTicketClicked();
        button.interactable = false;
    }
}
