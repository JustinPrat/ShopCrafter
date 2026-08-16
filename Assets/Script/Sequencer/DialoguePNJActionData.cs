using Sequencer.Actions;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueActionData", menuName = "Sequencer/DialogueActionData")]
public class DialoguePNJActionData : SequenceActionData
{
    public ManagerRefs ManagerRefs;
    public DialogueData DialogueData;

    public override SequenceActionBehavior CreateBehavior(GameObject owner)
    {
        DialoguePNJActionBehavior dialogueActionBehavior = new DialoguePNJActionBehavior();
        dialogueActionBehavior.Setup(this, owner);
        return dialogueActionBehavior;
    }

    public class DialoguePNJActionBehavior : SequenceActionBehavior
    {
        private DialoguePNJActionData data;        private PNJBrain pnjBrain;
        public void Setup(DialoguePNJActionData data, GameObject owner)
        {
            this.data = data;
            this.owner = owner;
            pnjBrain = owner.GetComponent<PNJBrain>();
        }

        public override IEnumerator Execute()
        {
            data.ManagerRefs.UIManager.ToggleDialoguePNJView(true, data.DialogueData, pnjBrain);
            yield return null;

            while (data.ManagerRefs.UIManager.DialogueView.ActiveState)
            {
                yield return null;
            }
        }

        public override void Stop()
        {
            data.ManagerRefs.UIManager.ToggleDialoguePNJView(false);
        }
    }
}
