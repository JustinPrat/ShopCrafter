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
    private Answer specialDialogue;

    public override QuestStepRuntime GetRuntimeLogic()
    {
        return new QuestStepTalkToPNJRuntime(new TalkToPNJData { PNJAmount = PNJAmount, NeedSpecialDialogue = needSpecialDialogue, SpecialDialogue = specialDialogue });
    }
}

public struct TalkToPNJData
{
    public int PNJAmount;
    public bool NeedSpecialDialogue;
    public Answer SpecialDialogue;
}

public class QuestStepTalkToPNJRuntime : QuestStepRuntime
{
    private TalkToPNJData talkToData;
    private Dictionary<PNJBrain, List<DialogueData>> pnjTalkedTo = new Dictionary<PNJBrain, List<DialogueData>>();

    public QuestStepTalkToPNJRuntime(TalkToPNJData data)
    {
        this.talkToData = data;
    }

    public override void InitializeQuestStep(string questId, int stepIndex, string questStepState, ManagerRefs managerRefs)
    {
        base.InitializeQuestStep(questId, stepIndex, questStepState, managerRefs);
        managerRefs.GameEventsManager.OnPNJTalked += OnPNJTalked;

        if (talkToData.NeedSpecialDialogue)
        {
            managerRefs.DialogueManager.SetSpecialDialogue(true, talkToData.SpecialDialogue);
        }
    }

    private void OnPNJTalked(PNJBrain brain, DialogueData data)
    {
        if (talkToData.NeedSpecialDialogue && talkToData.SpecialDialogue.AnswerDialogueData != null)
        {
            if (talkToData.SpecialDialogue.AnswerDialogueData == data && !pnjTalkedTo.ContainsKey(brain))
            {
                AddNewData(brain, data);
            }
        }
        else
        {
            if (!pnjTalkedTo.ContainsKey(brain))
            {
                AddNewData(brain, data);
            }
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
        managerRefs.GameEventsManager.OnPNJTalked -= OnPNJTalked;

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