using System.Collections;
using TriInspector;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "MovePositionActionData", menuName = "Sequencer/MovePositionActionData")]
    public class MovePositionActionData : SequenceActionData
    {
        public bool UseUI;
        public MoveType MovementType;
        public Vector3 Movement;
        public AnimationCurve MoveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration;

        public bool UseExecuteValue;

        [ShowIf(nameof(UseExecuteValue))]
        public Vector3 ExecuteValue;

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
            }

            public override IEnumerator Execute()
            {
                timer = 0;

                SetBasePos();
                targetPos = GetTargetPos(data.Movement);

                while (timer < data.Duration)
                {
                    timer += Time.unscaledDeltaTime;
                    float t = data.MoveCurve.Evaluate(timer / data.Duration);
                    SetTargetPos(t);
                    yield return null;
                }

                SetTargetPos(1);
            }

            public override void Stop()
            {
                SetTargetPos(1);
            }

            public override void SetExecuteBaseValue()
            {
                if (data.UseExecuteValue)
                {
                    targetPos = GetTargetPos(data.ExecuteValue);
                    SetTargetPos(1);
                }
            }

            private void SetBasePos()
            {
                switch (data.MovementType)
                {
                    case MoveType.Local:
                        if (data.UseUI)
                        {
                            RectTransform rect = owner.GetComponent<RectTransform>();
                            basePos = rect.anchoredPosition;
                        }
                        else
                        {
                            basePos = owner.transform.localPosition;
                        }
                        break;

                    case MoveType.World:
                    case MoveType.Offset:
                        basePos = owner.transform.position;
                        break;
                }
            }

            private void SetTargetPos(float t)
            {
                switch (data.MovementType)
                {
                    case MoveType.Local:
                        if (data.UseUI)
                        {
                            RectTransform rect = owner.GetComponent<RectTransform>();
                            rect.anchoredPosition = Vector3.LerpUnclamped(basePos, targetPos, t);
                        }
                        else
                        {
                            owner.transform.localPosition = Vector3.LerpUnclamped(basePos, targetPos, t);
                        }
                        break;

                    case MoveType.World:
                    case MoveType.Offset:
                        owner.transform.position = Vector3.LerpUnclamped(basePos, targetPos, t);
                        break;
                }
            }

            private Vector3 GetTargetPos(Vector3 movement)
            {
                switch (data.MovementType)
                {
                    case MoveType.Local:
                    case MoveType.World:
                        return movement;

                    case MoveType.Offset:
                        return basePos + movement;
                }

                return Vector3.zero;
            }
        }
    }
}

