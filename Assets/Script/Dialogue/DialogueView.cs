using Coffee.UIEffects;
using PrimeTween;
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
    private TextMeshProUGUI textName;

    [SerializeField] 
    private float delayBetweenSkip;

    [SerializeField]
    private float delayBeforeAnswerSelect = 0.1f;

    [SerializeField]
    private Image portrait;

    [SerializeField]
    private RectTransform answerParent;

    [SerializeField]
    private AnswerUIButton answerPrefab;

    [SerializeField]
    private UIEffect backGroundUIEffect;

    [SerializeField]
    private UIEffectPreset defaultBGUIPreset;

    [SerializeField]
    private float delayBeforeLeave = 1;

    [SerializeField]
    private Sequencer.Sequencer outAnim;

    [SerializeField]
    private Sequencer.Sequencer inQuestionAnim;

    [SerializeField]
    private Sequencer.Sequencer outQuestionAnim;

    [SerializeField]
    private DialogueBubbleUI dialogueBubbleUI;

    private DialogueData currentDialogue;
    private PNJBrain currentPNJ;
    private SpecialDialogue currentSpecialDialogue;
    private int currentDialogueIndex;
    private List<AnswerUIButton> answerUIButtons = new List<AnswerUIButton>();
    private List<DialogueBubbleUI> dialogueBubbles = new List<DialogueBubbleUI>();

    private bool isAsking = false;
    private float currentDelaySkip;
    private float currentDelayAnswerSelect;
    private bool isPNJTalking;
    private bool hasModifiedPreset;
    private bool isLeaving;
    private GameObject currentEmotion;

    private void Awake()
    {
        dialogueBubbleUI.TextWriter.OnTextEvent.AddListener(OnTextEvent);
    }

    private void OnDestroy()
    {
        dialogueBubbleUI.TextWriter.OnTextEvent.RemoveListener(OnTextEvent);
    }

    public void Setup (DialogueData dialogueData, PNJBrain pnjBehaviour)
    {
        isPNJTalking = true;
        currentDialogue = dialogueData;
        currentPNJ = pnjBehaviour;
        portrait.sprite = currentPNJ.Data.Identity.Portrait;
        textName.text = currentPNJ.Data.Identity.Name;
        StartDialogue();
    }

    public void Setup(DialogueData dialogueData, Identity identity)
    {
        isPNJTalking = false;
        currentDialogue = dialogueData;
        portrait.sprite = identity.Portrait;
        textName.text = identity.Name;
        StartDialogue();
    }

    protected override void OnInputDeviceChanged()
    {
        base.OnInputDeviceChanged();

        if (managerRefs.InputManager.IsGamepad && answerUIButtons.Count > 0)
        {
            StartCoroutine(SelectButtonAfterFrame(answerUIButtons[0].gameObject));
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void Update()
    {
        if (currentDelaySkip > 0f)
        {
            currentDelaySkip -= Time.unscaledDeltaTime;
        }

        if (currentDelayAnswerSelect > 0f)
        {
            currentDelayAnswerSelect -= Time.unscaledDeltaTime;
        }
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            managerRefs.InputManager.Actions.UI.Submit.started += OnNextDialogueStarted;
            managerRefs.InputManager.Actions.UI.Cancel.performed += OnCancelPerformed;
        }
        else
        {
            managerRefs.InputManager.Actions.UI.Submit.started -= OnNextDialogueStarted;
            managerRefs.InputManager.Actions.UI.Cancel.performed -= OnCancelPerformed;

            if (hasModifiedPreset)
            {
                backGroundUIEffect.LoadPreset(defaultBGUIPreset);
            }
        }

        hasModifiedPreset = false;
    }

    private void OnCancelPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isAsking)
        {
            foreach (AnswerUIButton button in answerUIButtons)
            {
                if (button.IsOutAnswer)
                {
                    button.Click();
                    break;
                }
            }
        }
        else
        {
            NextInputPressed();
        }
    }

    private void OnNextDialogueStarted (InputAction.CallbackContext ctx)
    {
        if (managerRefs.UIManager.CurrentFocus != this)
            return;

        NextInputPressed();
    }

    private void NextInputPressed()
    {
        if (!isAsking && currentDelaySkip <= 0f)
        {
            currentDelaySkip = delayBetweenSkip;

            if (dialogueBubbleUI.IsWriting)
            {
                dialogueBubbleUI.Skip();
            }
            else
            {
                NextLine();
            }
        }
    }

    private void OnTextEvent(TMPEventArgs args) 
    {
        if (isPNJTalking)
            currentPNJ.OnTextEvent(args);
    }

    private void StartDialogue ()
    {
        if (currentDialogue.EffectPreset != null && backGroundUIEffect != null)
        {
            hasModifiedPreset = true;
            backGroundUIEffect.LoadPreset(currentDialogue.EffectPreset);
        }

        answerParent.gameObject.SetActive(false);
        isAsking = false;
        currentDialogueIndex = 0;

        for (int i = answerUIButtons.Count - 1; i >= 0; i--)
        {
            Destroy(answerUIButtons[i].gameObject);
        }

        if (currentDialogue.ReplaceMainDialogue != null && isPNJTalking)
        {
            currentPNJ.ChangeMainDialogue(currentDialogue.ReplaceMainDialogue);
        }

        answerUIButtons.Clear();
        NextLine(0);

        if (isPNJTalking)
            managerRefs.GameEventsManager.OnPNJTalked?.Invoke(currentPNJ, currentDialogue);
    }

    private void TryAskQuestion ()
    {
        isAsking = true;

        if (managerRefs.DialogueManager.SpecialDialogues.Count > 0 && isPNJTalking)
        {
            SpecialDialogue specialDialogue = managerRefs.DialogueManager.GetSpecialDialogue(currentPNJ.gameObject);
            if (specialDialogue != null)
            {
                currentSpecialDialogue = specialDialogue;
                AnswerUIButton answer = Instantiate(answerPrefab, answerParent);
                answer.Setup(specialDialogue.Answers[0], false);
                answer.OnAnswerClicked += ChooseAnswer;

                answerUIButtons.Add(answer);
            }
        }

        if (currentDialogue.HasQuestion)
        {
            for (int i = 0; i < currentDialogue.Answers.Count; i++)
            {
                bool isLastAnswer = i >= currentDialogue.Answers.Count -1;

                AnswerUIButton answer = Instantiate(answerPrefab, answerParent);
                answer.Setup(currentDialogue.Answers[i], isLastAnswer);
                answer.OnAnswerClicked += ChooseAnswer;

                answerUIButtons.Add(answer);
            }
        }

        if (answerUIButtons.Count > 0)
        {
            currentDelayAnswerSelect = delayBeforeAnswerSelect;

            if (managerRefs.InputManager.IsGamepad)
            {
                StartCoroutine(SelectButtonAfterFrame(answerUIButtons[0].gameObject));
            }

            answerParent.gameObject.SetActive(true);
            SetupNavigation();

            outQuestionAnim.StopSequence();
            inQuestionAnim.StartSequence();
        }
        else
        {
            isAsking = false;
        }
    }

    private void SetupNavigation()
    {
        for (int i = 0; i < answerUIButtons.Count; i++)
        {
            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Explicit;

            if (i > 0)
                customNav.selectOnUp = answerUIButtons[i - 1].Selectable;

            if (i < answerUIButtons.Count - 1)
                customNav.selectOnDown = answerUIButtons[i + 1].Selectable;

            answerUIButtons[i].Selectable.navigation = customNav;
        }
    }

    private void ChooseAnswer (Answer selectedAnswer)
    {
        if (currentDelayAnswerSelect > 0)
            return;

        for (int i = answerUIButtons.Count - 1; i >= 0; i--)
        {
            Destroy(answerUIButtons[i].gameObject);
        }

        answerUIButtons.Clear();
        if (isPNJTalking && currentSpecialDialogue != null && currentSpecialDialogue.Answers.Contains(selectedAnswer))
        {
            managerRefs.DialogueManager.ConsumeSpecialDialogue(currentSpecialDialogue, currentPNJ.gameObject);
        }

        if (isPNJTalking && selectedAnswer.ReplaceMainDialogue != null)
        {
            currentPNJ.ChangeMainDialogue(selectedAnswer.ReplaceMainDialogue);
        }

        currentDialogue = selectedAnswer.AnswerDialogueData;
        if (selectedAnswer.Reward.Value != null)
        {
            selectedAnswer.Reward.Value.OnGetReward(managerRefs, isPNJTalking ? currentPNJ.gameObject : gameObject);
        }

        inQuestionAnim.StopSequence();
        outQuestionAnim.StartSequence();

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

            dialogueBubbleUI.SetText(currentDialogue.Lines[currentDialogueIndex].Line);

            if (currentEmotion != null)
            {
                Destroy(currentEmotion);
            }

            if (currentDialogue.Lines[currentDialogueIndex].Emotion != null)
            {
                currentEmotion = Instantiate(currentDialogue.Lines[currentDialogueIndex].Emotion.EmotionPrefab, portrait.transform);
            }

            if (currentDialogue.Lines[currentDialogueIndex].Reward.Value != null)
            {
                currentDialogue.Lines[currentDialogueIndex].Reward.Value.OnGetReward(managerRefs, isPNJTalking ? currentPNJ.gameObject : gameObject);
            }

            if (!isAsking)
                dialogueBubbleUI.SetClickVisual(true);
            else
                dialogueBubbleUI.SetClickVisual(false);
        }
    }

    public void StopDialogue(bool applyDelay = true)
    {
        if (applyDelay)
        {
            if (!isLeaving)
                StartCoroutine(LeaveAfterDelay());
        }
        else
        {
            CloseDialogue();
        }
    }

    IEnumerator LeaveAfterDelay()
    {
        if (outAnim != null)
            outAnim.StartSequence();

        isLeaving = true;
        yield return new WaitForSecondsRealtime(delayBeforeLeave);
        isLeaving = false;
        CloseDialogue();
    }

    public void CloseDialogue()
    {
        managerRefs.UIManager.ToggleDialoguePNJView(false);

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
