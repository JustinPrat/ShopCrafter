using System;
using UnityEngine;

public class MoneyGiverInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private string interactText = "";

    [SerializeField]
    private float delayBetweenGive = 0.2f;

    [SerializeField]
    private Sequencer.Sequencer sequenceGive;

    public bool IsLocked { get; set; }

    public string InteractText => interactText + (moneyNeeded - moneyGiven);

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }
    public Action<IInteractable> OnStartInteractEvent { get; set; }
    public Action<IInteractable> OnReleaseInteractEvent { get; set; }

    private int moneyGiven;
    private int moneyNeeded;
    private bool interactPressed;
    private float giveTimer;

    private void Start()
    {
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneMoneyRequired += OnMilestoneMoneyRequired;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneMoneyRequireStop += OnMilestoneMoneyRequireStop;
    }

    private void OnDestroy()
    {
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneMoneyRequired -= OnMilestoneMoneyRequired;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneMoneyRequireStop -= OnMilestoneMoneyRequireStop;

        OnDestroyEvent?.Invoke(this);
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

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return moneyNeeded > 0 && managerRefs.SellManager.CoinAmount >= 0;
    }

    public void StartInteract(PlayerBrain playerBrain)
    {
        interactPressed = true;
        giveTimer = delayBetweenGive;
        OnStartInteractEvent?.Invoke(this);
    }
    public void ReleaseInteract(PlayerBrain playerBrain)
    {
        interactPressed = false;
        OnReleaseInteractEvent?.Invoke(this);
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

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
        OnTargetedEvent?.Invoke(this);
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
        OnUnTargetedEvent?.Invoke(this);
    }
}
