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
            return new CinemachineChangePriorityBehavior(owner, this);
        }

        public class CinemachineChangePriorityBehavior : SequenceActionBehavior
        {
            private CinemachineChangePriorityData data;            private CinemachineCamera cinemachineCam;
            public CinemachineChangePriorityBehavior(GameObject owner, CinemachineChangePriorityData data) : base(owner)
            {
                this.data = data;
            }

            public override void Setup()
            {
                cinemachineCam = owner.GetComponent<CinemachineCamera>();
            }

            public override IEnumerator Execute()
            {
                cinemachineCam.Priority = data.Priority;
                yield return null;
            }
        }
    }
}
