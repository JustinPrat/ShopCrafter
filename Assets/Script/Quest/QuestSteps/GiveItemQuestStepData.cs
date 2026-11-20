using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/Quests/GiveItemQuestStepData")]
public class GiveItemQuestStepData : QuestStepData
{
    public SpecialDialogueData specialDialogueData;

    [SerializeField]
    private QuestStepDisplayData stepDisplayData;
    public override QuestStepDisplayData QuestStepUIDisplayData => stepDisplayData;

    public override QuestStepRuntime GetRuntimeLogic()
    {
        return new GiveItemQuestStepRuntime(this, new SpecialDialogue(specialDialogueData));
    }
}

public class GiveItemQuestStepRuntime : QuestStepRuntime
{    private GiveItemQuestStepData data;
    private SpecialDialogue specialDialogue;

    public GiveItemQuestStepRuntime(GiveItemQuestStepData data, SpecialDialogue specialDialogue)
    {
        this.data = data;
        this.specialDialogue = specialDialogue;
    }

    public override void InitializeQuestStep(string questId, int stepIndex, string questStepState, ManagerRefs managerRefs, PNJBrain owner)
    {
        base.InitializeQuestStep(questId, stepIndex, questStepState, managerRefs, owner);
        specialDialogue.Owner = owner.gameObject;

        managerRefs.DialogueManager.SetSpecialDialogue(true, specialDialogue);
        managerRefs.GameEventsManager.OnSpecialDialogueUsed += OnSpecialDialogueUsed;
    }

    private void OnSpecialDialogueUsed(SpecialDialogue dialogue)
    {
        if (dialogue == specialDialogue)
        {
            if (dialogue.Answers.Count <= 0)
            {
                FinishQuestStep();
            }
        }
    }

    protected override void FinishQuestStep()
    {
        managerRefs.GameEventsManager.OnSpecialDialogueUsed -= OnSpecialDialogueUsed;
        base.FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
    }
}