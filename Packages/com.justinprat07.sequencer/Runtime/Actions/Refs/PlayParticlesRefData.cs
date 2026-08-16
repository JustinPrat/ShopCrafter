using System;
using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [Serializable]
    public class PlayParticlesRefData : SequenceRefActionData
    {
        [SerializeField] 
        private ParticleSystem particleSystem;

        public override IEnumerator Execute()
        {
            particleSystem.Play();
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
