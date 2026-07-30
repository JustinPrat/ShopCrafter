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
            return new ActivationActionBehavior(owner, this);
        }

        public class ActivationActionBehavior : SequenceActionBehavior
        {
            private ActivationActionData data;
            public ActivationActionBehavior(GameObject owner, ActivationActionData data) : base(owner)
            {
                this.data = data;
            }

            public override void Setup()
            {
            }

            public override IEnumerator Execute()
            {
                owner.SetActive(data.IsActive);
                yield return null;
            }
        }
    }
}