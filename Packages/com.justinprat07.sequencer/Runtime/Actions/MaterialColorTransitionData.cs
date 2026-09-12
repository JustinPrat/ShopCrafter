using System.Collections;
using TriInspector;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "MaterialColorTransitionData", menuName = "Sequencer/MaterialColorTransitionData")]
    public class MaterialColorTransitionData : SequenceActionData
    {
        public string ColorPropertyName;
        public Color Target = Color.white;
        public AnimationCurve ColorCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration = 1f;

        public bool KeepBaseValueAtEnd;
        public bool UseExecuteValue;

        [ShowIf(nameof(UseExecuteValue))]
        public Color ExecuteValue;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            MaterialColorTransitionBehavior behavior = new MaterialColorTransitionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class MaterialColorTransitionBehavior : SequenceActionBehavior
        {
            private MaterialColorTransitionData data;
            private float timer;
            private Color baseColor;
            private Renderer renderer;

            public void Setup(MaterialColorTransitionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;
                renderer = owner.GetComponent<Renderer>();
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                baseColor = renderer.material.GetColor(data.ColorPropertyName);

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;
                    renderer.material.SetColor(data.ColorPropertyName, Color.LerpUnclamped(baseColor, data.Target, data.ColorCurve.Evaluate(timer / data.Duration)));
                    yield return null;
                }

                renderer.material.SetColor(data.ColorPropertyName, data.KeepBaseValueAtEnd ? baseColor : data.Target);
            }

            public override void Stop()
            {
                renderer.material.SetColor(data.ColorPropertyName, data.KeepBaseValueAtEnd ? baseColor : data.Target);
            }

            public override void SetExecuteBaseValue()
            {
                if (data.UseExecuteValue)
                    renderer.material.SetColor(data.ColorPropertyName, data.ExecuteValue);
            }
        }
    }
}