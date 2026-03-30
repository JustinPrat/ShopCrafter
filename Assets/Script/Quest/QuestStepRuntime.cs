public abstract class QuestStepRuntime
{
    protected bool isFinished = false;
    protected string questId;
    protected int stepIndex;
    protected ManagerRefs managerRefs;
    protected PNJBrain owner;

    public virtual void InitializeQuestStep(string questId, int stepIndex, string questStepState, ManagerRefs managerRefs, PNJBrain owner)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        this.managerRefs = managerRefs;
        this.owner = owner;

        if (questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }

    protected virtual void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            managerRefs.GameEventsManager.questEvents.AdvanceQuest(questId);
        }
    }

    protected virtual void ChangeState(string newState, string newStatus)
    {
        managerRefs.GameEventsManager.questEvents.QuestStepStateChange(
            questId, 
            stepIndex, 
            new QuestStepState(newState, newStatus)
        );
    }

    protected abstract void SetQuestStepState(string state);
}
