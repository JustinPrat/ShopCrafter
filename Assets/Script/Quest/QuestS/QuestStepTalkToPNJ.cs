using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/Quests/TalkToPNJ")]
public class QuestStepTalkToPNJ : QuestStepData
{
    [SerializeField]
    private int PNJAmount;

    [SerializeField]
    private bool needSpecialDialogue;

    [SerializeField, ShowIf(nameof(needSpecialDialogue))]
    private SpecialDialogueData specialDialogueData;

    public override QuestStepRuntime GetRuntimeLogic()
    {
        return new QuestStepTalkToPNJRuntime(new TalkToPNJData { PNJAmount = PNJAmount, NeedSpecialDialogue = needSpecialDialogue, SpecialDialogue = new SpecialDialogue(specialDialogueData) });
    }
}

public struct TalkToPNJData
{
    public int PNJAmount;
    public bool NeedSpecialDialogue;
    public SpecialDialogue SpecialDialogue;
}

public class QuestStepTalkToPNJRuntime : QuestStepRuntime
{
    private TalkToPNJData talkToData;
    private Dictionary<PNJBrain, List<DialogueData>> pnjTalkedTo = new Dictionary<PNJBrain, List<DialogueData>>();

    public QuestStepTalkToPNJRuntime(TalkToPNJData data)
    {
        this.talkToData = data;
    }

    public override void InitializeQuestStep(string questId, int stepIndex, string questStepState, ManagerRefs managerRefs, PNJBrain owner)
    {
        base.InitializeQuestStep(questId, stepIndex, questStepState, managerRefs, owner);

        if (talkToData.NeedSpecialDialogue)
        {
            SpecialDialogue specialDialogue = talkToData.SpecialDialogue;
            specialDialogue.Owner = owner.gameObject;

            managerRefs.DialogueManager.SetSpecialDialogue(true, specialDialogue);
            managerRefs.GameEventsManager.OnSpecialDialogueUsed += OnSpecialDialogueUsed;
        }
        else
        {
            managerRefs.GameEventsManager.OnPNJTalked += OnPNJTalked;
        }
    }

    private void OnSpecialDialogueUsed(SpecialDialogue specialDialogue)
    {
        if (specialDialogue == talkToData.SpecialDialogue)
        {
            if (specialDialogue.Answers.Count <= 0)
            {
                FinishQuestStep();
            }
        }
    }

    private void OnPNJTalked(PNJBrain brain, DialogueData data)
    {
        if (!pnjTalkedTo.ContainsKey(brain))
        {
            AddNewData(brain, data);
        }

        if (pnjTalkedTo.Keys.Count >= talkToData.PNJAmount)
        {
            FinishQuestStep();
        }
    }

    private void AddNewData(PNJBrain brain, DialogueData data)
    {
        pnjTalkedTo.TryAdd(brain, new List<DialogueData>());
        pnjTalkedTo[brain].Add(data);
    }

    protected override void FinishQuestStep()
    {
        if (talkToData.NeedSpecialDialogue)
        {
            managerRefs.GameEventsManager.OnSpecialDialogueUsed -= OnSpecialDialogueUsed;
        }
        else
        {
            managerRefs.GameEventsManager.OnPNJTalked -= OnPNJTalked;
        }

        if (talkToData.NeedSpecialDialogue)
        {
            managerRefs.DialogueManager.SetSpecialDialogue(false, talkToData.SpecialDialogue);
        }

        base.FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
    }
}