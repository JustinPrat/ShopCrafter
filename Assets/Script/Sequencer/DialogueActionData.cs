using Sequencer.Actions;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueActionData", menuName = "Sequencer/DialogueActionData")]
public class DialogueActionData : SequenceActionData
{
    public ManagerRefs ManagerRefs;
    public DialogueData DialogueData;

    public override SequenceActionBehavior CreateBehavior(GameObject owner)
    {
        return new DialogueActionBehavior(owner, this);
    }

    public class DialogueActionBehavior : SequenceActionBehavior
    {
        private DialogueActionData data;        private PNJBrain pnjBrain;
        public DialogueActionBehavior(GameObject owner, DialogueActionData data) : base(owner)
        {
            this.data = data;
        }

        public override void Setup()
        {
            pnjBrain = owner.GetComponent<PNJBrain>();
        }

        public override IEnumerator Execute()
        {
            data.ManagerRefs.UIManager.ToggleDialogueView(true, data.DialogueData, pnjBrain);

            while (data.ManagerRefs.UIManager.DialogueView.gameObject.activeInHierarchy)
            {
                yield return null;
            }
        }
    }
}
