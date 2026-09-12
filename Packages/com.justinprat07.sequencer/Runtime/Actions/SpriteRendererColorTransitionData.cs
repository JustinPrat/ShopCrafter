using System.Collections;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "SpriteRendererColorTransitionData", menuName = "Sequencer/SpriteRendererColorTransitionData")]
    public class SpriteRendererColorTransitionData : SequenceActionData
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
            SpriteRendererColorTransitionBehavior behavior = new SpriteRendererColorTransitionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class SpriteRendererColorTransitionBehavior : SequenceActionBehavior
        {
            private SpriteRendererColorTransitionData data;
            private float timer;
            private Color baseColor;
            private SpriteRenderer spriteRenderer;

            public void Setup(SpriteRendererColorTransitionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;
                spriteRenderer = owner.GetComponent<SpriteRenderer>();
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                baseColor = spriteRenderer.color;

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;
                    spriteRenderer.color = Color.LerpUnclamped(baseColor, data.Target, data.ColorCurve.Evaluate(timer / data.Duration));
                    yield return null;
                }

                spriteRenderer.color = data.KeepBaseValueAtEnd ? baseColor : data.Target;
            }

            public override void Stop()
            {
                spriteRenderer.color = data.KeepBaseValueAtEnd ? baseColor : data.Target;
            }

            public override void SetExecuteBaseValue()
            {
                if (data.UseExecuteValue)
                    spriteRenderer.color = data.ExecuteValue;
            }
        }
    }
}