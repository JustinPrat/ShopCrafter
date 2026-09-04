using System.Collections;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "ImageColorTransitionData", menuName = "Sequencer/ImageColorTransitionData")]
    public class ImageColorTransitionData : SequenceActionData
    {
        public Color Target = Color.white;
        public AnimationCurve ColorCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration = 1f;

        public bool KeepBaseValueAtEnd;
        public bool UseExecuteValue;

        [ShowIf(nameof(UseExecuteValue))]
        public Color ExecuteValue;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            ImageColorTransitionBehavior behavior = new ImageColorTransitionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class ImageColorTransitionBehavior : SequenceActionBehavior
        {
            private ImageColorTransitionData data;
            private float timer;
            private Color baseColor;
            private Image image;

            public void Setup(ImageColorTransitionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;
                image = owner.GetComponent<Image>();
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                baseColor = image.color;

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;
                    image.color = Color.LerpUnclamped(baseColor, data.Target, data.ColorCurve.Evaluate(timer / data.Duration));
                    yield return null;
                }

                image.color = data.KeepBaseValueAtEnd ? baseColor : data.Target;
            }

            public override void Stop()
            {
                image.color = data.KeepBaseValueAtEnd ? baseColor : data.Target;
            }

            public override void SetExecuteBaseValue()
            {
                if (data.UseExecuteValue)
                    image.color = data.ExecuteValue;
            }
        }
    }
}