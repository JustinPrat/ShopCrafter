using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "NestedSequenceActionData", menuName = "Sequencer/NestedSequenceActionData")]
    public class NestedSequenceActionData : SequenceActionData
    {
        public List<SequenceActionData> NestedActions;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            NestedSequenceActionBehavior nestedSequenceActionBehavior = new NestedSequenceActionBehavior();
            nestedSequenceActionBehavior.Setup(this, owner);
            return nestedSequenceActionBehavior;
        }

        public class NestedSequenceActionBehavior : SequenceActionBehavior
        {
            private NestedSequenceActionData data;
            private List<SequenceActionBehavior> behaviors = new List<SequenceActionBehavior>();
            private SequenceActionBehavior currentBehavior;

            public override IEnumerator Execute()
            {
                foreach (SequenceActionBehavior behavior in behaviors)
                {
                    currentBehavior = behavior;
                    yield return behavior.Execute();
                }

                currentBehavior = null;
            }

            public void Setup(NestedSequenceActionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;

                foreach (SequenceActionData action in data.NestedActions)
                {
                    SequenceActionBehavior behavior = action.CreateBehavior(owner);
                    behaviors.Add(behavior);
                }
            }

            public override void Stop()
            {
                if (currentBehavior != null)
                {
                    currentBehavior.Stop();
                }
            }
        }
    }
}
