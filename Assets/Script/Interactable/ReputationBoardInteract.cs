using System;
using UnityEngine;

public class ReputationBoardInteract : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private WorldSpeech worldSpeech;

    [SerializeField]
    private string reputationReadyText = "?";

    public bool IsLocked { get; set; }

    public string InteractText => "Reputation";

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }

    private bool milestoneReady;
    private bool isTargeted;

    private void Start()
    {
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneReady += OnMilestoneReady;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneReached += OnMilestoneReached;
    }

    private void OnMilestoneReached(int obj)
    {
        milestoneReady = false;
    }

    private void OnMilestoneReady()
    {
        milestoneReady = true;

        if (!isTargeted)
        {
            worldSpeech.DisplaySpeech(reputationReadyText, true);
        }
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);

        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneReady -= OnMilestoneReady;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneReached -= OnMilestoneReached;
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleReputationBoardView(true);
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
        if (milestoneReady)
        {
            worldSpeech.StopSpeech();
        }

        OnTargetedEvent?.Invoke(this);
        isTargeted = true;
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
        OnUnTargetedEvent?.Invoke(this);

        if (milestoneReady)
        {
            worldSpeech.DisplaySpeech(reputationReadyText, true);
        }

        isTargeted = false;
    }
}
