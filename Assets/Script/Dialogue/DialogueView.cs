using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPEffects.Components;
using TMPEffects.TMPEvents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueView : UIView
{
    [SerializeField]
    private TextMeshProUGUI textHolder;

    [SerializeField] 
    private TextMeshProUGUI textName;

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
    private PNJBrain currentPNJ;
    private int currentDialogueIndex;
    private List<AnswerUIButton> answerUIButtons = new List<AnswerUIButton>();

    private bool isAsking = false;

    public void Setup (DialogueData dialogueData, PNJBrain pnjBehaviour)
    {
        currentDialogue = dialogueData;
        currentPNJ = pnjBehaviour;

        portrait.sprite = currentPNJ.Data.Identity.Portrait;
        textName.text = currentPNJ.Data.Identity.Name;
        StartDialogue();

        textWriter.OnTextEvent.AddListener(OnTextEvent);
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            managerRefs.InputManager.Actions.Player.NextDialogue.started += OnNextDialogueStarted;
        }
        else
        {
            managerRefs.InputManager.Actions.Player.NextDialogue.started -= OnNextDialogueStarted;
        }
    }

    private void OnNextDialogueStarted (InputAction.CallbackContext ctx)
    {
        if (isAsking)
            return;

        NextLine();
    }

    private void OnTextEvent(TMPEventArgs args) 
    {
        currentPNJ.OnTextEvent(args);
    }

    private void StartDialogue ()
    {
        isAsking = false;
        currentDialogueIndex = 0;

        for (int i = answerUIButtons.Count - 1; i > 0; i--)
        {
            Destroy(answerUIButtons[i].gameObject);
        }

        answerUIButtons.Clear();

        NextLine(0);
    }

    private void AskQuestion ()
    {
        isAsking = true;
        for (int i = 0; i < currentDialogue.Answers.Count; i++)
        {
            AnswerUIButton answer = Instantiate(answerPrefab, answerParent);
            answer.Setup(currentDialogue.Answers[i]);
            answer.OnAnswerClicked += ChooseAnswer;

            answerUIButtons.Add(answer);
        }

        StartCoroutine(SelectButtonAfterFrame(answerUIButtons[0].gameObject));
    }

    private IEnumerator SelectButtonAfterFrame(GameObject gameObject)
    {
        yield return new WaitForEndOfFrame();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    private void ChooseAnswer (Answer selectedAnswer)
    {
        for (int i = answerUIButtons.Count - 1; i >= 0; i--)
        {
            Destroy(answerUIButtons[i].gameObject);
        }

        answerUIButtons.Clear();

        currentDialogue = selectedAnswer.NextDialogueData;
        if (selectedAnswer.Reward != null)
        {
            selectedAnswer.Reward.Value.OnGetReward(managerRefs, currentPNJ.gameObject);
        }

        if (currentDialogue != null)
        {
            StartDialogue();
        }
        else
        {
            StopDialogue();
        }
    }

    public void NextLine (int incrementIndex = 1)
    {
        currentDialogueIndex += incrementIndex;

        if (currentDialogueIndex > currentDialogue.Lines.Count - 1)
        {
            if (currentDialogue.HasNextDialogue)
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
            if (currentDialogueIndex >= currentDialogue.Lines.Count - 1)
            {
                if (currentDialogue.HasQuestion)
                {
                    AskQuestion();
                }
            }

            textAnimator.SetText(currentDialogue.Lines[currentDialogueIndex].Line);
        }
    }

    private void StopDialogue ()
    {
        managerRefs.UIManager.ToggleDialogueView(false);
    }
}
