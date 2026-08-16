using Sequencer.Actions;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace SequencerCinemachine
{
    [CreateAssetMenu(fileName = "CinemachineChangePriorityData", menuName = "Sequencer/CinemachineChangePriorityData")]
    public class CinemachineChangePriorityData : SequenceActionData
    {
        public int Priority = 10;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            CinemachineChangePriorityBehavior cinemachineChangePriorityBehavior = new CinemachineChangePriorityBehavior();
            cinemachineChangePriorityBehavior.Setup(this, owner);
            return cinemachineChangePriorityBehavior;
        }

        public class CinemachineChangePriorityBehavior : SequenceActionBehavior
        {
            private CinemachineChangePriorityData data;            private CinemachineCamera cinemachineCam;

            public void Setup(CinemachineChangePriorityData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;
                cinemachineCam = owner.GetComponent<CinemachineCamera>();
            }

            public override IEnumerator Execute()
            {
                cinemachineCam.Priority = data.Priority;
                yield return null;
            }

            public override void Stop()
            {
            }
        }
    }
}
