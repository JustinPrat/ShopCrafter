using System.Collections;
using TriInspector;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "MaterialFloatTransitionData", menuName = "Sequencer/MaterialFloatTransitionData")]
    public class MaterialFloatTransitionData : SequenceActionData
    {
        public string FloatPropertyName;
        public float Target = 1;
        public AnimationCurve FloatCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration = 1f;

        public bool KeepBaseValueAtEnd;
        public bool UseExecuteValue;

        [ShowIf(nameof(UseExecuteValue))]
        public float ExecuteValue;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            MaterialFloatTransitionBehavior behavior = new MaterialFloatTransitionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class MaterialFloatTransitionBehavior : SequenceActionBehavior
        {
            private MaterialFloatTransitionData data;
            private float timer;
            private float baseValue;
            private Renderer renderer;

            public void Setup(MaterialFloatTransitionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;
                renderer = owner.GetComponent<Renderer>();
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                baseValue = renderer.material.GetFloat(data.FloatPropertyName);

                while (timer < data.Duration)
                {
                    timer += Time.deltaTime;
                    renderer.material.SetFloat(data.FloatPropertyName, Mathf.LerpUnclamped(baseValue, data.Target, data.FloatCurve.Evaluate(timer / data.Duration)));
                    yield return null;
                }

                renderer.material.SetFloat(data.FloatPropertyName, data.KeepBaseValueAtEnd ? baseValue : data.Target);
            }

            public override void SetExecuteBaseValue()
            {
                renderer.material.SetFloat(data.FloatPropertyName, data.KeepBaseValueAtEnd ? baseValue : data.Target);
            }

            public override void Stop()
            {
                if (data.UseExecuteValue)
                    renderer.material.SetFloat(data.FloatPropertyName, data.ExecuteValue);            }
        }
    }
}