using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "ScaleActionData", menuName = "Sequencer/ScaleActionData")]
    public class ScaleActionData : SequenceActionData
    {
        public Vector3 ScaleTarget;
        public AnimationCurve ScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration = 1f;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            return new ScaleActionBehavior(owner, this);
        }

        public class ScaleActionBehavior : SequenceActionBehavior
        {
            private ScaleActionData data;
            private float timer;
            private Vector3 baseScale;
            public ScaleActionBehavior(GameObject owner, ScaleActionData data) : base(owner)
            {
                this.data = data;
            }

            public override void Setup()
            {
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                baseScale = owner.transform.localScale;

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;
                    owner.transform.localScale = Vector3.LerpUnclamped(baseScale, data.ScaleTarget, data.ScaleCurve.Evaluate(timer / data.Duration));
                    yield return null;
                }

                owner.transform.localScale = data.ScaleTarget;
            }
        }
    }
}