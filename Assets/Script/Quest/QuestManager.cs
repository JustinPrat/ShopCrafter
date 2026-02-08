using Alchemy.Inspector;
using Alchemy.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TNRD;
using UnityEngine;

[AlchemySerialize]
public partial class QuestManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] 
    private bool loadQuestState = true;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private List<QuestInfoSO> quests;

    [AlchemySerializeField, NonSerialized, DisableInEditMode]
    private Dictionary<string, Quest> questMap;

#if UNITY_EDITOR
    [Button]
    private void UpdateQuests()
    {
        quests.Clear();
        quests.AddRange(Resources.LoadAll<QuestInfoSO>("Data/Quests"));
    }

    [Button]
    private void StartQuestEditor(QuestInfoSO questInfoSO)
    {
        StartQuest(questInfoSO.ID);
    }

#endif

    private void Awake()
    {
        managerRefs.QuestManager = this;
        questMap = CreateQuestMap();
    }

    private void OnDestroy()
    {
        managerRefs.GameEventsManager.questEvents.onStartQuest -= StartQuest;
        managerRefs.GameEventsManager.questEvents.onAdvanceQuest -= AdvanceQuest;
        managerRefs.GameEventsManager.questEvents.onFinishQuest -= FinishQuest;
        managerRefs.GameEventsManager.questEvents.onQuestStepStateChange -= QuestStepStateChange;
    }

    private void Start()
    {
        managerRefs.GameEventsManager.questEvents.onStartQuest += StartQuest;
        managerRefs.GameEventsManager.questEvents.onAdvanceQuest += AdvanceQuest;
        managerRefs.GameEventsManager.questEvents.onFinishQuest += FinishQuest;
        managerRefs.GameEventsManager.questEvents.onQuestStepStateChange += QuestStepStateChange;

        foreach (Quest quest in questMap.Values)
        {
            // initialize any loaded quest steps
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            // broadcast the initial state of all quests on startup
            managerRefs.GameEventsManager.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        managerRefs.GameEventsManager.questEvents.QuestStateChange(quest);
    }

    private void StartQuest(string id) 
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.ID, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            if (quest.info.NeedFinishInteraction)
            {
                ChangeQuestState(quest.info.ID, QuestState.CAN_FINISH);
            }
            else
            {
                FinishQuest(quest.info.ID);
            }
        }
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.ID, QuestState.FINISHED);
    }

    private void ClaimRewards(Quest quest)
    {
        foreach (SerializableInterface<IRewardable> reward in quest.info.Rewards)
        {
            if (reward.Value != null)
            {
                reward.Value.OnGetReward(managerRefs);
            }
        }
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in quests)
        {
            if (idToQuestMap.ContainsKey(questInfo.ID))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.ID);
            }
            idToQuestMap.Add(questInfo.ID, LoadQuest(questInfo));
        }
        return idToQuestMap;
    }

    public Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try 
        {
            QuestData questData = quest.GetQuestData();
            // serialize using JsonUtility, but use whatever you want here (like JSON.NET)
            string serializedData = JsonUtility.ToJson(questData);
            // saving to PlayerPrefs is just a quick example for this tutorial video,
            // you probably don't want to save this info there long-term.
            // instead, use an actual Save & Load system and write to a file, the cloud, etc..
            PlayerPrefs.SetString(quest.info.ID, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.ID + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try
        {
            if (PlayerPrefs.HasKey(questInfo.ID) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.ID);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates, managerRefs);
            }
            else 
            {
                quest = new Quest(questInfo, managerRefs);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.ID + ": " + e);
        }
        return quest;
    }
}
