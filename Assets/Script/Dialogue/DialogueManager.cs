using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    private List<SpecialDialogue> specialDialogues = new List<SpecialDialogue>();

    public List<SpecialDialogue> SpecialDialogues => specialDialogues;
    private void Awake()
    {
        managerRefs.DialogueManager = this;
    }

    public void StartDialogue (DialogueData data, PNJBrain pnjBrain)
    {
        managerRefs.UIManager.ToggleDialogueView(true, data, pnjBrain);
    }

    public void SetSpecialDialogue(bool isAdded, SpecialDialogue specialDialogue)
    {
        if (isAdded)
        {
            if (!specialDialogues.Contains(specialDialogue))
            {
                specialDialogues.Add(specialDialogue);
            }
        }
        else if (specialDialogue != null && specialDialogues.Contains(specialDialogue))
        {
            specialDialogues.Remove(specialDialogue);
        }
    }

    public Answer ConsumeSpecialDialogue(SpecialDialogue specialDialogue, GameObject consumer)
    {
        Answer answer = specialDialogue.Answers[0];
        specialDialogue.Answers.RemoveAt(0);
        specialDialogue.AskedPNJs.Add(consumer);

        if (specialDialogue.Answers.Count <= 0)
        {
            SetSpecialDialogue(false, specialDialogue);
        }

        managerRefs.GameEventsManager.OnSpecialDialogueUsed?.Invoke(specialDialogue);
        return answer;
    }

    public SpecialDialogue GetSpecialDialogue(GameObject consumer)
    {
        foreach (SpecialDialogue specialDialogue in specialDialogues)
        {
            if (specialDialogue.Owner != consumer && !specialDialogue.AskedPNJs.Contains(consumer))
            {
                return specialDialogue;
            }
        }

        return null;
    }
}

[Serializable]
public struct SpecialDialogueData
{
    public List<Answer> Answers;
}

[Serializable]
public class SpecialDialogue
{
    public List<Answer> Answers { get; set; }
    public GameObject Owner { get; set; }
    public List<GameObject> AskedPNJs { get; set; } = new List<GameObject>();

    public SpecialDialogue(SpecialDialogueData data)
    {
        this.Answers = new List<Answer>(data.Answers);
    }
}
