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
            return new NestedSequenceActionBehavior(owner, this);
        }

        public class NestedSequenceActionBehavior : SequenceActionBehavior
        {
            private NestedSequenceActionData data;

            public NestedSequenceActionBehavior(GameObject owner, NestedSequenceActionData data) : base(owner)
            {
                this.data = data;
            }

            public override IEnumerator Execute()
            {
                foreach (SequenceActionData action in data.NestedActions)
                {
                    SequenceActionBehavior behavior = action.CreateBehavior(owner);
                    yield return behavior.Execute();
                }
            }

            public override void Setup()
            {
            }
        }
    }
}
