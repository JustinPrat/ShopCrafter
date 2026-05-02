using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.TMPEvents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueView : UIView
{
    [SerializeField]
    private Transform dialogueBubblesAnchor;

    [SerializeField] 
    private TextMeshProUGUI textName;

    [SerializeField]
    private float bubbleSpacing;

    [SerializeField] 
    private float delayBetweenSkip;

    [SerializeField]
    private GameObject dialogueBubbleUIPrefab;

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

    [SerializeField]
    private List<float> transparencySteps;

    private DialogueData currentDialogue;
    private PNJBrain currentPNJ;
    private SpecialDialogue currentSpecialDialogue;
    private int currentDialogueIndex;
    private List<AnswerUIButton> answerUIButtons = new List<AnswerUIButton>();
    private List<DialogueBubbleUI> dialogueBubbles = new List<DialogueBubbleUI>();

    private bool isAsking = false;
    private float currentDelaySkip;

    public void Setup (DialogueData dialogueData, PNJBrain pnjBehaviour)
    {
        currentDialogue = dialogueData;
        currentPNJ = pnjBehaviour;

        portrait.sprite = currentPNJ.Data.Identity.Portrait;
        textName.text = currentPNJ.Data.Identity.Name;
        StartDialogue();

        //textWriter.OnTextEvent.AddListener(OnTextEvent);
    }

    private void Update()
    {
        if (currentDelaySkip > 0f)
        {
            currentDelaySkip -= Time.unscaledDeltaTime;
        }
    }

    private DialogueBubbleUI CreateBubble()
    {
        DialogueBubbleUI bubble = Instantiate(dialogueBubbleUIPrefab, dialogueBubblesAnchor).GetComponent<DialogueBubbleUI>();
        dialogueBubbles.Add(bubble);
        bubble.TextWriter.OnTextEvent.AddListener(OnTextEvent);
        bubble.SetTransparency(0f);

        return bubble;
    }

    private IEnumerator UpdateBubbleStep()
    {
        yield return new WaitForEndOfFrame();

        DialogueBubbleUI lastBubble = dialogueBubbles[dialogueBubbles.Count - 1];
        lastBubble.UpdateBubbleHeight();

        Vector2 basePos = lastBubble.MainRectTransform.anchoredPosition;
        lastBubble.SetTransparency(1f, 0.5f);
        lastBubble.MainRectTransform.anchoredPosition = new Vector2(lastBubble.MainRectTransform.anchoredPosition.x, lastBubble.MainRectTransform.anchoredPosition.y - lastBubble.BubbleHeight - bubbleSpacing);
        lastBubble.MainRectTransform.DOAnchorPos(basePos, 0.5f).SetUpdate(true);

        for (int i = dialogueBubbles.Count - 1; i >= 0; i--)
        {
            DialogueBubbleUI dialogueBubble = dialogueBubbles[i];
            int currentStep = dialogueBubbles.Count - 1 - i;

            if (dialogueBubbles.Count - 1 == i)
                continue;

            if (currentStep > transparencySteps.Count - 1)
            {
                Destroy(dialogueBubble.gameObject);
            }
            else
            {
                dialogueBubble.SetTransparency(transparencySteps[currentStep - 1], 0.5f);
                dialogueBubble.MainRectTransform.DOAnchorPosY(dialogueBubble.MainRectTransform.anchoredPosition.y + lastBubble.BubbleHeight + bubbleSpacing, 0.5f).SetUpdate(true);
            }
        }
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            managerRefs.InputManager.Actions.Player.NextDialogue.started += OnNextDialogueStarted;
            managerRefs.InputManager.Actions.UI.Cancel.performed += OnCancelPerformed;
        }
        else
        {
            managerRefs.InputManager.Actions.Player.NextDialogue.started -= OnNextDialogueStarted;
            managerRefs.InputManager.Actions.UI.Cancel.performed -= OnCancelPerformed;
        }
    }

    private void OnCancelPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        managerRefs.UIManager.ToggleDialogueView(false);
    }

    private void OnNextDialogueStarted (InputAction.CallbackContext ctx)
    {
        if (isAsking || currentDelaySkip > 0f)
            return;

        currentDelaySkip = delayBetweenSkip;
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
        if (currentDialogue.HasQuestion)
        {
            for (int i = 0; i < currentDialogue.Answers.Count; i++)
            {
                AnswerUIButton answer = Instantiate(answerPrefab, answerParent);
                answer.Setup(currentDialogue.Answers[i]);
                answer.OnAnswerClicked += ChooseAnswer;

                answerUIButtons.Add(answer);
            }
        }

        if (managerRefs.DialogueManager.SpecialDialogues.Count > 0)
        {
            SpecialDialogue specialDialogue = managerRefs.DialogueManager.GetSpecialDialogue(currentPNJ.gameObject);
            if (specialDialogue != null)
            {
                currentSpecialDialogue = specialDialogue;
                AnswerUIButton answer = Instantiate(answerPrefab, answerParent);
                answer.Setup(specialDialogue.Answers[0]);
                answer.OnAnswerClicked += ChooseAnswer;

                answerUIButtons.Add(answer);
            }
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
        if (currentSpecialDialogue != null && currentSpecialDialogue.Answers.Contains(selectedAnswer))
        {
            managerRefs.DialogueManager.ConsumeSpecialDialogue(currentSpecialDialogue, currentPNJ.gameObject);
        }

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


            DialogueBubbleUI bubble = CreateBubble();
            bubble.SetText(currentDialogue.Lines[currentDialogueIndex].Line);
            StartCoroutine(UpdateBubbleStep());
        }
    }

    public void StopDialogue ()
    {
        managerRefs.UIManager.ToggleDialogueView(false);

        for (int i = dialogueBubbles.Count - 1; i >= 0; i--)
        {
            DialogueBubbleUI dialogueBubble = dialogueBubbles[i];
            if (dialogueBubble != null)
            {
                Destroy(dialogueBubble.gameObject);
            }
        }

        dialogueBubbles.Clear();
    }
}
