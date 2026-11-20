using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    public QuestInfoSO Info;
    public QuestState State;

    private PNJBrain currentOwner;
    private int currentQuestStepIndex;
    private QuestStepState[] questStepStates;
    private List<QuestStepRuntime> questStepRuntimes = new List<QuestStepRuntime>();
    private ManagerRefs managerRefs;

    public Quest(QuestInfoSO questInfo, ManagerRefs managerRefs)
    {
        this.Info = questInfo;
        this.State = QuestState.NOT_RECEIVED;
        this.currentQuestStepIndex = 0;
        this.managerRefs = managerRefs;
        this.questStepStates = new QuestStepState[Info.QuestSteps.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates, ManagerRefs managerRefs)
    {
        this.Info = questInfo;
        this.State = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;
        this.managerRefs = managerRefs;

        // if the quest step states and prefabs are different lengths,
        // something has changed during development and the saved data is out of sync.
        if (this.questStepStates.Length != this.Info.QuestSteps.Length)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are "
                + "of different lengths. This indicates something changed "
                + "with the QuestInfo and the saved data is now out of sync. "
                + "Reset your data - as this might cause issues. QuestId: " + this.Info.ID);
        }
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < Info.QuestSteps.Length);
    }

    public void StartQuest(PNJBrain owner)
    {
        currentOwner = owner;
    }

    public void InstantiateCurrentQuestStep()
    {
        QuestStepData questStepData = GetCurrentQuestStepData();
        if (questStepData != null)
        {
            QuestStepRuntime questStep = questStepData.GetRuntimeLogic();
            questStep.InitializeQuestStep(Info.ID, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state, managerRefs, currentOwner);
            questStepRuntimes.Add(questStep);
        }
    }

    public QuestStepData GetCurrentQuestStepData()
    {
        QuestStepData questStepData = null;
        if (CurrentStepExists())
        {
            questStepData = Info.QuestSteps[currentQuestStepIndex];
        }
        else 
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that "
                + "there's no current step: QuestId=" + Info.ID + ", stepIndex=" + currentQuestStepIndex);
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
                + "Quest Id = " + Info.ID + ", Step Index = " + stepIndex);
        }
    }

    public QuestData GetQuestData()
    {
        return new QuestData(State, currentQuestStepIndex, questStepStates);
    }

    public string GetFullStatusText()
    {
        string fullStatus = "";

        if (State == QuestState.NOT_RECEIVED)
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
            if (State == QuestState.CAN_FINISH)
            {
                fullStatus += "The quest is ready to be turned in.";
            }
            else if (State == QuestState.FINISHED)
            {
                fullStatus += "The quest has been completed!";
            }
        }

        return fullStatus;
    }
}
