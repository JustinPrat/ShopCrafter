using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Quest
{
    public QuestInfoSO info;

    public QuestState state;
    private int currentQuestStepIndex;
    private QuestStepState[] questStepStates;
    private List<QuestStepRuntime> questStepRuntimes = new List<QuestStepRuntime>();
    private ManagerRefs managerRefs;
    public Quest(QuestInfoSO questInfo, ManagerRefs managerRefs)
    {
        this.info = questInfo;
        this.state = QuestState.NOT_RECEIVED;
        this.currentQuestStepIndex = 0;
        this.managerRefs = managerRefs;
        this.questStepStates = new QuestStepState[info.QuestSteps.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates, ManagerRefs managerRefs)
    {
        this.info = questInfo;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;
        this.managerRefs = managerRefs;

        // if the quest step states and prefabs are different lengths,
        // something has changed during development and the saved data is out of sync.
        if (this.questStepStates.Length != this.info.QuestSteps.Length)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are "
                + "of different lengths. This indicates something changed "
                + "with the QuestInfo and the saved data is now out of sync. "
                + "Reset your data - as this might cause issues. QuestId: " + this.info.ID);
        }
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.QuestSteps.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        QuestStepData questStepData = GetCurrentQuestStepData();
        if (questStepData != null)
        {
            QuestStepRuntime questStep = questStepData.GetRuntimeLogic();
            questStep.InitializeQuestStep(info.ID, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state, managerRefs);
            questStepRuntimes.Add(questStep);
        }
    }

    private QuestStepData GetCurrentQuestStepData()
    {
        QuestStepData questStepData = null;
        if (CurrentStepExists())
        {
            questStepData = info.QuestSteps[currentQuestStepIndex];
        }
        else 
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that "
                + "there's no current step: QuestId=" + info.ID + ", stepIndex=" + currentQuestStepIndex);
        }
        return questStepData;
    }

    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].state = questStepState.state;
            questStepStates[stepIndex].status = questStepState.status;
        }
        else 
        {
            Debug.LogWarning("Tried to access quest step data, but stepIndex was out of range: "
                + "Quest Id = " + info.ID + ", Step Index = " + stepIndex);
        }
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, currentQuestStepIndex, questStepStates);
    }

    public string GetFullStatusText()
    {
        string fullStatus = "";

        if (state == QuestState.NOT_RECEIVED)
        {
            fullStatus = "This quest can be started!";
        }
        else 
        {
            // display all previous quests with strikethroughs
            for (int i = 0; i < currentQuestStepIndex; i++)
            {
                fullStatus += "<s>" + questStepStates[i].status + "</s>\n";
            }
            // display the current step, if it exists
            if (CurrentStepExists())
            {
                fullStatus += questStepStates[currentQuestStepIndex].status;
            }
            // when the quest is completed or turned in
            if (state == QuestState.CAN_FINISH)
            {
                fullStatus += "The quest is ready to be turned in.";
            }
            else if (state == QuestState.FINISHED)
            {
                fullStatus += "The quest has been completed!";
            }
        }

        return fullStatus;
    }
}
