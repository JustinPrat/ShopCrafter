using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "RotationActionData", menuName = "Sequencer/RotationActionData")]
    public class RotationActionData : SequenceActionData
    {
        public Vector3 RotationTarget;
        public AnimationCurve RotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration = 1f;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            return new RotationActionBehavior(owner, this);
        }

        public class RotationActionBehavior : SequenceActionBehavior
        {
            private RotationActionData data;
            private float timer;
            private Vector3 baseRotation;
            public RotationActionBehavior(GameObject owner, RotationActionData data) : base(owner)
            {
                this.data = data;
            }

            public override void Setup()
            {
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                baseRotation = owner.transform.localRotation.eulerAngles;

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;

                    float t = data.RotationCurve.Evaluate(timer / data.Duration);
                    Vector3 rotation = new Vector3()
                    {
                        x = Mathf.LerpAngle(baseRotation.x, data.RotationTarget.x, t),
                        y = Mathf.LerpAngle(baseRotation.y, data.RotationTarget.y, t),
                        z = Mathf.LerpAngle(baseRotation.z, data.RotationTarget.z, t)
                    };

                    owner.transform.localRotation = Quaternion.Euler(rotation);
                    yield return null;
                }

                owner.transform.localRotation = Quaternion.Euler(data.RotationTarget);
            }
        }
    }
}