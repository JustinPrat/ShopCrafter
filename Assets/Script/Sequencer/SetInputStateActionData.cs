using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "SetInputStateActionData", menuName = "Sequencer/SetInputStateActionData")]
    public class SetInputStateActionData : SequenceActionData
    {
        public ManagerRefs ManagerRefs;
        public PlayerInputType PlayerInputType;
        public UIInputType UIInputType;
        public bool State;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            SetInputStateActionBehavior behavior = new SetInputStateActionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class SetInputStateActionBehavior : SequenceActionBehavior
        {
            private SetInputStateActionData data;

            public void Setup(SetInputStateActionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;
            }

            public override IEnumerator Execute()
            {
                data.ManagerRefs.InputManager.SetAction(data.PlayerInputType, data.State);
                data.ManagerRefs.InputManager.SetAction(data.UIInputType, data.State);
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