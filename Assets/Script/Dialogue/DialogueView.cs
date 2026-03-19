using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    [SerializeField]
    private GameObject closeButton;

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
        closeButton.SetActive(false);

        for (int i = answerUIButtons.Count - 1; i >= 0; i--)
        {
            Destroy(answerUIButtons[i].gameObject);
        }

        if (currentDialogue.ReplaceMainDialogue != null)
        {
            currentPNJ.ChangeMainDialogue(currentDialogue.ReplaceMainDialogue);
        }

        answerUIButtons.Clear();
        NextLine(0);
        managerRefs.GameEventsManager.OnPNJTalked?.Invoke(currentPNJ, currentDialogue);
    }

    private void TryAskQuestion ()
    {
        isAsking = true;
        for (int i = 0; i < currentDialogue.Answers.Count; i++)
        {
            AnswerUIButton answer = Instantiate(answerPrefab, answerParent);
            answer.Setup(currentDialogue.Answers[i]);
            answer.OnAnswerClicked += ChooseAnswer;

            answerUIButtons.Add(answer);
        }

        if (managerRefs.DialogueManager.SpecialDialogues.Count > 0)
        {
            KeyValuePair<DialogueData, Answer> specialDialogue = managerRefs.DialogueManager.SpecialDialogues.First();

            AnswerUIButton answer = Instantiate(answerPrefab, answerParent);
            answer.Setup(specialDialogue.Value);
            answer.OnAnswerClicked += ChooseAnswer;

            answerUIButtons.Add(answer);
        }

        if (answerUIButtons.Count > 0)
        {
            StartCoroutine(SelectButtonAfterFrame(answerUIButtons[0].gameObject));
            closeButton.SetActive(true);
        }
        else
        {
            isAsking = false;
        }
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
        managerRefs.DialogueManager.SetSpecialDialogue(false, selectedAnswer);

        if (selectedAnswer.ReplaceMainDialogue != null)
        {
            currentPNJ.ChangeMainDialogue(selectedAnswer.ReplaceMainDialogue);
        }

        currentDialogue = selectedAnswer.AnswerDialogueData;
        if (selectedAnswer.Reward.Value != null)
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
                TryAskQuestion();
            }

            textAnimator.SetText(currentDialogue.Lines[currentDialogueIndex].Line);
        }
    }

    public void StopDialogue ()
    {
        managerRefs.UIManager.ToggleDialogueView(false);
    }
}
