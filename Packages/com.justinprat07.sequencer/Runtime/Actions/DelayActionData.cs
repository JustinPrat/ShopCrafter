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
            DelayActionBehavior delayActionBehavior = new DelayActionBehavior();
            delayActionBehavior.Setup(this, owner);
            return delayActionBehavior;
        }

        public class DelayActionBehavior : SequenceActionBehavior
        {
            private DelayActionData data;            private WaitForSeconds delay;

            public void Setup(DelayActionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;

                delay = new WaitForSeconds(data.Value);
            }

            public override IEnumerator Execute()
            {
                yield return delay;
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