using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "NestedSequenceActionData", menuName = "Sequencer/NestedSequenceActionData")]
    public class NestedSequenceActionData : SequenceActionData
    {
        public bool PlayAtSameTime;

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
            private MonoBehaviour monoBehaviour;

            public override IEnumerator Execute()
            {
                foreach (SequenceActionBehavior behavior in behaviors)
                {
                    currentBehavior = behavior;

                    if (data.PlayAtSameTime)
                    {
                        monoBehaviour.StartCoroutine(behavior.Execute());
                    }
                    else
                    {
                        yield return behavior.Execute();
                    }
                }

                currentBehavior = null;
            }

            public override void SetExecuteBaseValue()
            {
                foreach (SequenceActionBehavior behavior in behaviors)
                {
                    behavior.SetExecuteBaseValue();
                }
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

                monoBehaviour = owner.GetComponent<MonoBehaviour>();
                if (monoBehaviour == null)
                    monoBehaviour = owner.AddComponent<BehaviorCoroutine>();
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
