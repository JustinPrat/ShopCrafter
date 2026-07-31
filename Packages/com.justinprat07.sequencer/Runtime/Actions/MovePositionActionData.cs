using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "MovePositionActionData", menuName = "Sequencer/MovePositionActionData")]
    public class MovePositionActionData : SequenceActionData
    {
        public Vector3 MoveOffset;
        public AnimationCurve MoveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            return new MovePositionActionBehavior(owner, this);
        }

        public class MovePositionActionBehavior : SequenceActionBehavior
        {
            private MovePositionActionData data;
            private float timer;
            private Vector3 basePos;

            public MovePositionActionBehavior(GameObject owner, MovePositionActionData data) : base(owner) 
            {
                this.data = data;
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                basePos = owner.transform.position;

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;
                    owner.transform.position = Vector3.LerpUnclamped(basePos, basePos + data.MoveOffset, data.MoveCurve.Evaluate(timer / data.Duration));
                    yield return null;
                }

                owner.transform.position = basePos + data.MoveOffset;
            }

            public override void Setup()
            {
                timer = 0;
            }
        }
    }
}

