using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.TMPAnimations;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ShopCrafter/Dialogue/DialogueData")]
[DeclareFoldoutGroup("texttool", Title = "Text Tool")]
[DeclareTabGroup("texttool/addremove")]
public class DialogueData : ScriptableObject
{
    public List<LineData> Lines = new List<LineData>();
    public bool HasQuestion;
    public bool HasNextDialogue;

    [TriInspector.ShowIf(nameof(HasQuestion))]
    public List<Answer> Answers = new List<Answer>();

    [TriInspector.ShowIf(nameof(HasNextDialogue))]
    public DialogueData NextDialogue;


#if UNITY_EDITOR
    [Group("texttool")]
    public TMPAnimationDatabase TMPAnimDatabase;

    public enum TMPType
    {
        Animation,
        Event
    }

    public enum Position
    {
        FullText,
        Front,
        Back
    }

    [SerializeField, Group("texttool"), EnumToggleButtons()]
    private TMPType tmpType = TMPType.Animation;

    [Button(), Group("texttool/addremove"), Tab("ADD")]
    public void AddEffect (int lineIndex, Position addedPosition)
    {
        if (lineIndex < Lines.Count)
        {
            LineData line = Lines[lineIndex];
            string elementNameFront = "";
            string elementNameBack = "";

            if (tmpType == TMPType.Event)
            {
                elementNameFront = "";
                elementNameBack = "<?" + TMPEventName.ToString() + ">";
            }
            else if (tmpType == TMPType.Animation)
            {
                elementNameFront = "<" + TMPAnimationName + ">";
                elementNameBack = "</" + TMPAnimationName + ">";
            }

            switch (addedPosition)
            {
                case Position.Front:
                    line.Line = elementNameFront + elementNameBack + line.Line;
                    break;
                case Position.Back:
                    line.Line = line.Line + elementNameFront + elementNameBack;
                    break;
                case Position.FullText:
                    line.Line = elementNameFront + line.Line + elementNameBack;
                    break;
            }

            Lines[lineIndex] = line;
        }
    }

    [Button(), Group("texttool/addremove"), Tab("REMOVE")]
    public void RemoveEffect(int lineIndex)
    {
        if (lineIndex < Lines.Count)
        {
            LineData line = Lines[lineIndex];
            string elementNameFront = "";
            string elementNameBack = "";

            if (tmpType == TMPType.Event)
            {
                elementNameBack = "<?" + TMPEventName.ToString() + ">";
            }
            else if (tmpType == TMPType.Animation)
            {
                elementNameFront = "<" + TMPAnimationName + ">";
                elementNameBack = "</" + TMPAnimationName + ">";
                line.Line = line.Line.Replace(elementNameFront, "");
            }

            line.Line = line.Line.Replace(elementNameBack, "");
            Lines[lineIndex] = line;
        }
    }

    [Dropdown(nameof(GetTMPAnimNames)), Group("texttool"), PropertySpace(SpaceAfter = 10), HideIf(nameof(TMPTypeIsEvent))]
    public string TMPAnimationName;

    [Group("texttool"), PropertySpace(SpaceAfter = 10), ShowIf(nameof(TMPTypeIsEvent))]
    public TMPEvents TMPEventName;

    private bool TMPTypeIsEvent () => tmpType == TMPType.Event;

    private string[] GetTMPAnimNames ()
    {
        if (TMPAnimDatabase == null)
            return new string[0];

        var type = TMPAnimDatabase.BasicAnimationDatabase.GetType();

        FieldInfo field = type.GetField("animations",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (field != null)
        {
            Dictionary<string, TMPAnimation> dictionnaire = field.GetValue(TMPAnimDatabase.BasicAnimationDatabase) as Dictionary<string, TMPAnimation>;

            if (dictionnaire != null) 
            {
                var keys = dictionnaire.Keys;
                return new List<string>(keys).ToArray();
            }
        }

        return new string[0];
    }


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