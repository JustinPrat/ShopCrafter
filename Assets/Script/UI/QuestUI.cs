using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questNameText;

    [SerializeField]
    private TextMeshProUGUI questStepDescText;

    public void Setup(Quest quest)
    {
        questNameText.text = quest.Info.DisplayName;
        questStepDescText.text = quest.GetCurrentQuestStepData().QuestStepUIDisplayData.Description;
    }
}
