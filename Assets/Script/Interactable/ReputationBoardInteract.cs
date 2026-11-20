using UnityEngine;

public class ReputationBoardInteract : WorldViewInteract<ReputationBoardView>
{
    [SerializeField]
    private WorldSpeech worldSpeech;

    [SerializeField]
    private string reputationReadyText = "?";
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneReady -= OnMilestoneReady;
        managerRefs.GameEventsManager.milestoneEvents.OnMilestoneReached -= OnMilestoneReached;
    }

    public override void StartInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleReputationBoardView(true);
        base.StartInteract(playerBrain);
    }

    public override void OnTargeted(PlayerBrain playerBrain)
    {
        if (milestoneReady)
        {
            worldSpeech.StopSpeech();
        }

        base.OnTargeted(playerBrain);
        isTargeted = true;
    }

    public override void UnTargeted(PlayerBrain playerBrain)
    {
        base.UnTargeted(playerBrain);

        if (milestoneReady)
        {
            worldSpeech.DisplaySpeech(reputationReadyText, true);
        }

        isTargeted = false;
    }
}
