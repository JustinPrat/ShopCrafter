using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    private Dictionary<DialogueData, Answer> specialDialogues = new Dictionary<DialogueData, Answer>();

    public Dictionary<DialogueData, Answer> SpecialDialogues => specialDialogues;

    private void Awake()
    {
        managerRefs.DialogueManager = this;
    }

    public void StartDialogue (DialogueData data, PNJBrain pnjBrain)
    {
        managerRefs.UIManager.ToggleDialogueView(true, data, pnjBrain);
    }

    public void SetSpecialDialogue(bool isAdded, Answer specialDialogue)
    {
        if (isAdded)
        {
            if (!specialDialogues.ContainsKey(specialDialogue.AnswerDialogueData))
            {
                specialDialogues.Add(specialDialogue.AnswerDialogueData, specialDialogue);
            }
        }
        else
        {
            specialDialogues.Remove(specialDialogue.AnswerDialogueData);
        }
    }
}
