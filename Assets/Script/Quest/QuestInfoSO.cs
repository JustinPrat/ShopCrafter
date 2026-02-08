using System.Collections;
using System.Collections.Generic;
using TNRD;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject, IRewardable
{
    [field: SerializeField] public string ID { get; private set; }

    [Header("General")]
    public string DisplayName;
    public bool NeedFinishInteraction;

    [Header("Steps")]
    public QuestStepData[] QuestSteps;

    [Header("Rewards")]
    public List<SerializableInterface<IRewardable>> Rewards;
    public DialogueData FinishedDialogueData;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ID = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver)
    {
        managerRefs.GameEventsManager.questEvents.StartQuest(ID);

        if (giver != null)
        {
            PNJBrain pnj = giver.GetComponent<PNJBrain>();
            if (pnj != null)
            {
                pnj.GiveQuest(this);
            }
        }
    }
}
