using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    public abstract class SequenceActionData : ScriptableObject
    {
        public abstract SequenceActionBehavior CreateBehavior(GameObject owner);

        public abstract class SequenceActionBehavior
        {
            protected GameObject owner;

            public SequenceActionBehavior(GameObject owner)
            {
                this.owner = owner;
            }

            public abstract void Setup();
            public abstract IEnumerator Execute();
        }
    }
}