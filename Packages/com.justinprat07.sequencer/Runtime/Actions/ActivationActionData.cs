using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "ActivationActionData", menuName = "Sequencer/ActivationActionData")]
    public class ActivationActionData : SequenceActionData
    {
        public bool IsActive;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            ActivationActionBehavior activationActionBehavior = new ActivationActionBehavior();
            activationActionBehavior.Setup(owner, this);
            return activationActionBehavior;
        }

        public class ActivationActionBehavior : SequenceActionBehavior
        {
            private ActivationActionData data;

            public void Setup(GameObject owner, ActivationActionData data)
            {
                this.data = data;
                this.owner = owner;
            }

            public override IEnumerator Execute()
            {
                owner.SetActive(data.IsActive);
                yield return null;
            }

            public override void Stop()
            {
            }

            public override void SetExecuteBaseValue()
            {
            }
        }
    }
}