using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "DialogueDataActionData", menuName = "Sequencer/DialogueDataActionData")]
    public class DialogueDataActionData : SequenceActionData
    {
        public ManagerRefs ManagerRefs;
        public DialogueData DialogueData;
        public IdentityData IdentityData;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            DialogueDataActionBehavior behavior = new DialogueDataActionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class DialogueDataActionBehavior : SequenceActionBehavior
        {
            private DialogueDataActionData data;

            public void Setup(DialogueDataActionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;
            }

            public override IEnumerator Execute()
            {
                data.ManagerRefs.UIManager.ToggleDialogueDataView(true, data.DialogueData, data.IdentityData.GetIdentity());
                yield return null;

                while (data.ManagerRefs.UIManager.DialogueView.ActiveState)
                {
                    yield return null;
                }
            }

            public override void Stop()
            {                data.ManagerRefs.UIManager.ToggleDialoguePNJView(false);            }
        }
    }
}