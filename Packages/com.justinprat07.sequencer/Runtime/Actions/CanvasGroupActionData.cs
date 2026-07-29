using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "CanvasGroupActionData", menuName = "Sequencer/CanvasGroupActionData")]
    public class CanvasGroupActionData : SequenceActionData
    {
        [Range(0f, 1f)]
        public float TargetValue = 1;

        public float Duration = 1;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            return new CanvasGroupActionBehavior(owner, this);
        }

        public class CanvasGroupActionBehavior : SequenceActionBehavior
        {
            private CanvasGroupActionData data;            private CanvasGroup canvasGroup;            private float timer;            private float baseValue;
            public CanvasGroupActionBehavior(GameObject owner, CanvasGroupActionData data) : base(owner)
            {
                this.data = data;
            }

            public override void Setup()
            {
                canvasGroup = owner.GetComponent<CanvasGroup>();
            }

            public override IEnumerator Execute()
            {
                baseValue = canvasGroup.alpha;

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(baseValue, data.TargetValue , data.Curve.Evaluate(timer/data.Duration));
                    yield return null;
                }
            }
        }
    }
}