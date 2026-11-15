using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "SetBlackBarsActivationActionData", menuName = "Sequencer/SetBlackBarsActivationActionData")]
    public class SetBlackBarsActivationActionData : SequenceActionData
    {
        public ManagerRefs ManagerRefs;
        public bool State;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            SetBlackBarsActivationActionBehavior behavior = new SetBlackBarsActivationActionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class SetBlackBarsActivationActionBehavior : SequenceActionBehavior
        {
            private SetBlackBarsActivationActionData data;

            public void Setup(SetBlackBarsActivationActionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;
            }

            public override IEnumerator Execute()
            {
                data.ManagerRefs.UIManager.ToggleBlackBarsView(data.State);
                yield return null;
            }

            public override void SetExecuteBaseValue()
            {
            }

            public override void Stop()
            {            }
        }
    }
}