using System.Collections.Generic;
using System.IO;
using TMPEffects.Components;
using TMPEffects.TMPEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : UIView
{
    [SerializeField]
    private TextMeshProUGUI textHolder;

    [SerializeField]
    private TMPWriter textWriter;

    [SerializeField]
    private TMPAnimator textAnimator;

    [SerializeField]
    private Image portrait;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private RectTransform answerParent;

    [SerializeField]
    private AnswerUIButton answerPrefab;

    private DialogueData currentDialogue;
    private PNJBehaviour currentPNJ;
    private int currentDialogueIndex;
    private List<AnswerUIButton> answerUIButtons = new List<AnswerUIButton>();

    public void Setup (DialogueData dialogueData, PNJBehaviour pnjBehaviour)
    {
        currentDialogue = dialogueData;
        currentPNJ = pnjBehaviour;

        portrait.sprite = currentPNJ.PNJData.Portrait;
        StartDialogue();

        textWriter.OnTextEvent.AddListener(OnTextEvent);
    }

    private void OnTextEvent(TMPEventArgs args) 
    {
        currentPNJ.OnTextEvent(args);
    }

    private void StartDialogue ()
    {
        currentDialogueIndex = 0;
        textAnimator.SetText(currentDialogue.Lines[currentDialogueIndex].Line);
    }

    private void AskQuestion ()
    {
        for (int i = 0; i < currentDialogue.Answers.Count; i++)
        {
            AnswerUIButton answer = Instantiate(answerPrefab, answerParent);
            answer.Setup(currentDialogue.Answers[i]);
            answer.OnAnswerClicked += ChooseAnswer;

            answerUIButtons.Add(answer);
        }
    }

    private void ChooseAnswer (Answer selectedAnswer)
    {
        for (int i = answerUIButtons.Count - 1; i >= 0; i--)
        {
            Destroy(answerUIButtons[i].gameObject);
        }

        answerUIButtons.Clear();

        currentDialogue = selectedAnswer.NextDialogueData;
        StartDialogue();
    }

    private void NextLine ()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex > currentDialogue.Lines.Count - 1)
        {
            if (currentDialogue.HasQuestion)
            {
                AskQuestion();
            }
            else if (currentDialogue.HasNextDialogue)
            {
                currentDialogue = currentDialogue.NextDialogue;
                StartDialogue();
            }
            else
            {
                StopDialogue();
            }
        }
        else
        {
            textAnimator.SetText(currentDialogue.Lines[currentDialogueIndex].Line);
        }
    }

    private void StopDialogue ()
    {
        managerRefs.UIManager.ToggleDialogueView(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            NextLine();
        }
    }
}
