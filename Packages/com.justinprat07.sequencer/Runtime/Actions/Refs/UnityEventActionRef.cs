using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;

namespace Sequencer.Actions
{
    [Serializable]
    public class UnityEventActionRef : SequenceRefActionData
    {
        [SerializeField]
        private UnityEvent unityEvent;

        public override IEnumerator Execute()
        {
            unityEvent?.Invoke();
            yield return null;
        }

        public override void SetExecuteBaseValue()
        {
        }

        public override void Stop()
        {
        }
    }
}
