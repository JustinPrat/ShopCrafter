using System;
using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    public abstract class SequenceActionData : ScriptableObject
    {
        public abstract SequenceActionBehavior CreateBehavior(GameObject owner);
    }

    [Serializable]
    public abstract class SequenceActionBehavior
    {
        protected GameObject owner;

        public abstract IEnumerator Execute();
        public abstract void Stop();
        public abstract void SetExecuteBaseValue();
    }

    [Serializable]
    public abstract class SequenceRefActionData : SequenceActionBehavior
    {
        public void CreateBehavior(GameObject owner)
        {
            this.owner = owner;
        }
    }
}