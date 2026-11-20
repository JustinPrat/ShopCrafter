using UnityEngine;

public class MoneyGiverInteract : BaseWorldInteract
{
    [SerializeField]
    private float delayBetweenGive = 0.2f;

    [SerializeField]
    private Sequencer.Sequencer sequenceGive;

    public override string InteractText => interactText + (moneyNeeded - moneyGiven);

    private int moneyGiven;
    private int moneyNeeded;
    private bool interactPressed;
    private float giveTimer;

    private void Start()
    {
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneMoneyRequired += OnMilestoneMoneyRequired;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneMoneyRequireStop += OnMilestoneMoneyRequireStop;
    }

    protected override void OnDestroy()
    {
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneMoneyRequired -= OnMilestoneMoneyRequired;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneMoneyRequireStop -= OnMilestoneMoneyRequireStop;

        base.OnDestroy();
    }

    private void OnMilestoneMoneyRequired(int amount)
    {
        moneyGiven = 0;
        moneyNeeded = amount;
    }

    private void OnMilestoneMoneyRequireStop()
    {
        moneyNeeded = 0;
        moneyGiven = 0;
        interactPressed = false;
    }

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        return moneyNeeded > 0 && managerRefs.SellManager.CoinAmount >= 0;
    }

    public override void StartInteract(PlayerBrain playerBrain)
    {
        interactPressed = true;
        giveTimer = delayBetweenGive;
        base.StartInteract(playerBrain);
    }
    public override void ReleaseInteract(PlayerBrain playerBrain)
    {
        interactPressed = false;
        base.ReleaseInteract(playerBrain);
    }

    private void Update()
    {
        giveTimer -= Time.unscaledDeltaTime;
        if (interactPressed && giveTimer <= 0)
        {
            GiveMoney();
        }
    }

    private void GiveMoney()
    {
        giveTimer = delayBetweenGive;
        moneyGiven++;

        managerRefs.SellManager.TryPayForItem(1);
        managerRefs.GameEventsManager.milestoneEvents.MilestoneGiveMoney(1);
    }
}
