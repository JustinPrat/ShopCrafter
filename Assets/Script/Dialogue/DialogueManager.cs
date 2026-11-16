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
        managerRefs.UIManager.ToggleDialoguePNJView(true, data, pnjBrain);
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
        PNJBrain pnjBrain = consumer.GetComponent<PNJBrain>();
        foreach (SpecialDialogue specialDialogue in specialDialogues)
        {
            bool isOwner = consumer == specialDialogue.Owner;
            bool canTargetSelf = specialDialogue.TargetSelf && isOwner;
            bool canTargetType = specialDialogue.ContainsTarget(pnjBrain.Data.PNJType) && !isOwner;

            if ((canTargetSelf || canTargetType) && !specialDialogue.AskedPNJs.Contains(consumer))
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
    public bool TargetSelf;
    public PNJType PNJTarget;
    public List<Answer> Answers;
}

[Serializable]
public class SpecialDialogue
{
    public bool TargetSelf;
    public PNJType PNJTarget;
    public List<Answer> Answers { get; set; }
    public GameObject Owner { get; set; }
    public List<GameObject> AskedPNJs { get; set; } = new List<GameObject>();

    public SpecialDialogue(SpecialDialogueData data)
    {
        this.Answers = new List<Answer>(data.Answers);
        PNJTarget = data.PNJTarget;
        TargetSelf = data.TargetSelf;
    }

    public bool ContainsTarget(PNJType pnjType)
    {
        return (PNJTarget & pnjType) != 0;
    }
}

[Serializable, Flags]
public enum PNJType
{
    Consumer = 1 << 1,
    Permanent = 1 << 2
}
