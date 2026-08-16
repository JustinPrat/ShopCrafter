using System.Collections;
using UnityEngine;
using static Sequencer.Actions.ActivationActionData;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "MovePositionActionData", menuName = "Sequencer/MovePositionActionData")]
    public class MovePositionActionData : SequenceActionData
    {
        public MoveType MovementType;
        public Vector3 Movement;
        public AnimationCurve MoveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration;

        public enum MoveType
        {
            Local,
            World,
            Offset
        }

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            MovePositionActionBehavior movePositionActionBehavior = new MovePositionActionBehavior();
            movePositionActionBehavior.Setup(this, owner);
            return movePositionActionBehavior;
        }

        public class MovePositionActionBehavior : SequenceActionBehavior
        {
            private MovePositionActionData data;
            private float timer;
            private Vector3 basePos;
            private Vector3 targetPos;

            public void Setup(MovePositionActionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;
                timer = 0;
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                basePos = owner.transform.position;

                switch (data.MovementType)
                {
                    case MoveType.Local:
                        targetPos = owner.transform.parent.position + data.Movement;
                        break;
                    case MoveType.World:
                        targetPos = data.Movement;
                        break;
                    case MoveType.Offset:
                        targetPos = basePos + data.Movement;
                        break;
                }

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;
                    owner.transform.position = Vector3.LerpUnclamped(basePos, targetPos, data.MoveCurve.Evaluate(timer / data.Duration));
                    yield return null;
                }

                owner.transform.position = targetPos;
            }

            public override void Stop()
            {
                owner.transform.position = targetPos;
            }
        }
    }
}

