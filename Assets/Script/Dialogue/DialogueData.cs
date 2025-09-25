using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ShopCrafter/Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public List<LineData> Lines = new List<LineData>();
    public bool HasQuestion;
    public bool HasNextDialogue;

    [ShowIf(nameof(HasQuestion))]
    public List<Answer> Answers = new List<Answer>();

    [ShowIf(nameof(HasNextDialogue))]
    public DialogueData NextDialogue;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < Answers.Count; i++)
        {
            Answer answer = Answers[i];
            if (answer.reward != null && answer.reward is not IRewardable)
            {
                answer.reward = null;
                Answers[i] = answer;
            }
        }
    }
#endif
}

[Serializable]
public struct LineData
{
    public string Line;
    public EmotionData Emotion;
}

[Serializable]
public struct Answer
{
    public string Line;
    public DialogueData NextDialogueData;
    public ScriptableObject reward;
}