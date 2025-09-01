using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    private void Awake()
    {
        managerRefs.DialogueManager = this;
    }

    public void StartDialogue (DialogueData data, PNJBehaviour pnjBehaviour)
    {
        managerRefs.UIManager.ToggleDialogueView(true, data, pnjBehaviour);
    }
}
