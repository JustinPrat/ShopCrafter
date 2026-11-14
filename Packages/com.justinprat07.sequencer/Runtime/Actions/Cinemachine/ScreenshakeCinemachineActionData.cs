using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "ScreenshakeCinemachineActionData", menuName = "Sequencer/ScreenshakeCinemachineActionData")]
    public class ScreenshakeCinemachineActionData : SequenceActionData
    {
        public float ImpulsePower = 0.5f;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            ScreenshakeCinemachineActionBehavior behavior = new ScreenshakeCinemachineActionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class ScreenshakeCinemachineActionBehavior : SequenceActionBehavior
        {   
            private ScreenshakeCinemachineActionData data;
            private CinemachineImpulseSource cinemachineImpulseSource;

            public void Setup(ScreenshakeCinemachineActionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;

                cinemachineImpulseSource = owner.GetComponent<CinemachineImpulseSource>();
            }

            public override IEnumerator Execute()
            {
                cinemachineImpulseSource.GenerateImpulse(data.ImpulsePower);
                yield return null;
            }

            public override void SetExecuteBaseValue()
            {
            }

            public override void Stop()
            {            }
        }
    }
}