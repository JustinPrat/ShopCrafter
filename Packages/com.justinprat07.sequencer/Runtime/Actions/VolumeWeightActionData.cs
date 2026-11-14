using System.Collections;
using TriInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "VolumeWeightActionData", menuName = "Sequencer/VolumeWeightActionData")]
    public class VolumeWeightActionData : SequenceActionData
    {
        [Range(0f, 1f)]
        public float TargetValue = 1;

        public float Duration = 1;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        public bool UseExecuteValue;

        [ShowIf(nameof(UseExecuteValue))]
        public float ExectuteValue = 0;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            VolumeWeightActionBehavior behavior = new VolumeWeightActionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class VolumeWeightActionBehavior : SequenceActionBehavior
        {
            private VolumeWeightActionData data;
            private Volume volumeComponent;            private float timer;            private float baseValue;

            public void Setup(VolumeWeightActionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;
                volumeComponent = owner.GetComponent<Volume>();
            }

            public override IEnumerator Execute()
            {
                baseValue = volumeComponent.weight;
                timer = 0f;

                while (timer < data.Duration)
                {
                    timer += Time.unscaledDeltaTime;
                    volumeComponent.weight = Mathf.Lerp(baseValue, data.TargetValue, data.Curve.Evaluate(timer / data.Duration));
                    yield return null;
                }

                volumeComponent.weight = data.TargetValue;
            }

            public override void SetExecuteBaseValue()
            {
                if (data.UseExecuteValue)
                {
                    volumeComponent.weight = data.ExectuteValue;
                }
            }

            public override void Stop()
            {                volumeComponent.weight = data.TargetValue;            }
        }
    }
}