using TMPEffects.Components;
using TMPro;
using UnityEngine;

public class DialogueView : UIView
{
    [SerializeField]
    private TextMeshProUGUI textHolder;

    [SerializeField]
    private TMPEffectComponent textEffectComponent;

    [SerializeField]
    private TMPAnimator textAnimator;

    private DialogueData currentDialogue;
    private PNJData currentPNJ;
    private int currentDialogueIndex;

    public void Setup (DialogueData dialogueData, PNJData pnjData)
    {
        currentDialogue = dialogueData;
        currentPNJ = pnjData;
        StartDialogue();
    }

    private void StartDialogue ()
    {
        currentDialogueIndex = 0;
        textAnimator.SetText(currentDialogue.Lines[currentDialogueIndex].Line);
    }

    private void AskQuestion ()
    {

    }

    private void NextLine ()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= currentDialogue.Lines.Count - 1)
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

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            NextLine();
        }
    }
}
