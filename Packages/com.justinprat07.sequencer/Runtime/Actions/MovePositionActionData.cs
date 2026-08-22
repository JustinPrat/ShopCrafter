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
                basePos = owner.transform.position;
                targetPos = GetTargetPos(data.Movement);

                while (timer < data.Duration)
                {
                    timer += Time.unscaledDeltaTime;
                    owner.transform.position = Vector3.LerpUnclamped(basePos, targetPos, data.MoveCurve.Evaluate(timer / data.Duration));
                    yield return null;
                }

                owner.transform.position = targetPos;
            }

            public override void Stop()
            {
                owner.transform.position = targetPos;
            }

            public override void SetExecuteBaseValue()
            {
                if (data.UseExecuteValue)
                    owner.transform.position = GetTargetPos(data.ExecuteValue);
            }

            private Vector3 GetTargetPos(Vector3 movement)
            {
                switch (data.MovementType)
                {
                    case MoveType.Local:
                        if (data.UseUI)
                        {
                            RectTransform rect = owner.GetComponent<RectTransform>();
                            RectTransform parentRect = rect.parent as RectTransform;

                            Vector2 anchorCenter = (rect.anchorMin + rect.anchorMax) * 0.5f;

                            Vector3 anchorLocalPos = new Vector3(
                                Mathf.Lerp(parentRect.rect.xMin, parentRect.rect.xMax, anchorCenter.x),
                                Mathf.Lerp(parentRect.rect.yMin, parentRect.rect.yMax, anchorCenter.y),
                                0f
                            );

                            return rect.parent.TransformPoint(anchorLocalPos + movement);
                        }
                        else
                        {
                            return owner.transform.parent.position + movement;
                        }

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

