using System.Collections;
using TriInspector;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "RotationActionData", menuName = "Sequencer/RotationActionData")]
    public class RotationActionData : SequenceActionData
    {
        public Vector3 RotationTarget;
        public AnimationCurve RotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration = 1f;

        public bool UseExecuteValue;
        [ShowIf(nameof(UseExecuteValue))]
        public Vector3 ExecuteValue;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            RotationActionBehavior rotationActionBehavior = new RotationActionBehavior();
            rotationActionBehavior.Setup(this, owner);
            return rotationActionBehavior;
        }

        public class RotationActionBehavior : SequenceActionBehavior
        {
            private RotationActionData data;
            private float timer;
            private Vector3 baseRotation;

            public void Setup(RotationActionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;
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

            public override void Stop()
            {
                owner.transform.localRotation = Quaternion.Euler(data.RotationTarget);
            }

            public override void SetExecuteBaseValue()
            {
                if (data.UseExecuteValue)
                    owner.transform.localRotation = Quaternion.Euler(data.ExecuteValue);
            }
        }
    }
}