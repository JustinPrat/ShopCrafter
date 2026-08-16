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
            CanvasGroupActionBehavior canvasGroupActionBehavior = new CanvasGroupActionBehavior();
            canvasGroupActionBehavior.Setup(this, owner);
            return canvasGroupActionBehavior;
        }

        public class CanvasGroupActionBehavior : SequenceActionBehavior
        {
            private CanvasGroupActionData data;            private CanvasGroup canvasGroup;            private float timer;            private float baseValue;

            public void Setup(CanvasGroupActionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;
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

                canvasGroup.alpha = data.TargetValue;
            }

            public override void Stop()
            {
                canvasGroup.alpha = data.TargetValue;
            }
        }
    }
}