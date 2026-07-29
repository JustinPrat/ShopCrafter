using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "DelayActionData", menuName = "Sequencer/DelayActionData")]
    public class DelayActionData : SequenceActionData
    {
        public float Value;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            return new DelayActionBehavior(owner, this);
        }

        public class DelayActionBehavior : SequenceActionBehavior
        {
            private DelayActionData data;            private WaitForSeconds delay;
            public DelayActionBehavior(GameObject owner, DelayActionData data) : base(owner)
            {
                this.data = data;
            }

            public override void Setup()
            {
                delay = new WaitForSeconds(data.Value);
            }

            public override IEnumerator Execute()
            {
                yield return delay;
            }
        }
    }
}