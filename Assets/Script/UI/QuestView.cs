using System.Collections.Generic;
using UnityEngine;

public class QuestView : UIView
{
    [SerializeField]
    private QuestUI questUIPrefab;

    [SerializeField]
    private Transform questHolder;

    [SerializeField]
    private Sequencer.Sequencer moveInQuest;

    [SerializeField]
    private Sequencer.Sequencer moveOutQuest;

    private Dictionary<Quest, QuestUI> questDataUIs = new Dictionary<Quest, QuestUI>();

    private void Start()
    {
        managerRefs.GameEventsManager.questEvents.OnQuestStateChange += OnQuestStateChange;
        managerRefs.GameEventsManager.questEvents.OnQuestStepComplete += OnQuestStepComplete;

        managerRefs.InputManager.Actions.Player.Quest.started += OnQuestInputStarted;
        managerRefs.InputManager.Actions.Player.Quest.canceled += OnQuestInputCanceled;

        moveOutQuest.StartSequence();
    }

    private void OnQuestInputStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        moveOutQuest.StopSequence();
        moveInQuest.StartSequence();
    }

    private void OnQuestInputCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        moveInQuest.StopSequence();
        moveOutQuest.StartSequence();
    }

    private void OnDestroy()
    {
        managerRefs.GameEventsManager.questEvents.OnQuestStateChange -= OnQuestStateChange;
        managerRefs.GameEventsManager.questEvents.OnQuestStepComplete -= OnQuestStepComplete;
    }

    private void OnQuestStepComplete(Quest quest)
    {
        UpdateQuestUI(quest);
    }

    private void OnQuestStateChange(Quest quest)
    {
        switch (quest.State)
        {
            case QuestState.NOT_RECEIVED:
                break;

            case QuestState.IN_PROGRESS:
                CreateQuestUI(quest);
                break;

            case QuestState.CAN_FINISH:
                break;

            case QuestState.FINISHED:
                DestroyQuestUI(quest);
                break;
        }
    }

    private QuestUI CreateQuestUI(Quest quest)
    {
        QuestUI questUI = Instantiate(questUIPrefab.gameObject, questHolder).GetComponent<QuestUI>();
        questUI.Setup(quest);
        questDataUIs.Add(quest, questUI);
        return questUI;
    }

    private void UpdateQuestUI(Quest quest)
    {
        foreach (KeyValuePair<Quest, QuestUI> questDataUIPair in questDataUIs)
        {
            if (questDataUIPair.Key == quest)
            {
                questDataUIPair.Value.Setup(quest);
                break;
            }
        }
    }

    private void DestroyQuestUI(Quest quest)
    {
        foreach (KeyValuePair<Quest, QuestUI> questDataUIPair in questDataUIs)
        {
            if (questDataUIPair.Key == quest)
            {
                Destroy(questDataUIPair.Value.gameObject);
                questDataUIs.Remove(quest);
                break;
            }
        }
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);
    }
}
