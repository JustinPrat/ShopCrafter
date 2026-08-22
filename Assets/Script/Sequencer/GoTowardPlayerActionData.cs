using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "GoTowardPlayerActionData", menuName = "Sequencer/GoTowardPlayerActionData")]
    public class GoTowardPlayerActionData : SequenceActionData
    {
        public ManagerRefs ManagerRefs;
        public AnimationCurve MoveCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float Duration;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            GoTowardPlayerActionBehavior behavior = new GoTowardPlayerActionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class GoTowardPlayerActionBehavior : SequenceActionBehavior
        {
            private GoTowardPlayerActionData data;
            private float timer;
            private Vector3 basePos;

            public void Setup(GoTowardPlayerActionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;
                timer = 0;
            }

            public override IEnumerator Execute()
            {
                timer = 0;
                basePos = owner.transform.position;
                Vector3 targetPos = data.ManagerRefs.PlayerManager.PlayerBrain.transform.position;

                while (timer < data.Duration)
                {
                    timer += Time.unscaledDeltaTime;
                    owner.transform.position = Vector3.LerpUnclamped(basePos, targetPos, data.MoveCurve.Evaluate(timer / data.Duration));
                    yield return null;
                }

                owner.transform.position = targetPos;
            }

            public override void SetExecuteBaseValue()
            {
            }

            public override void Stop()
            {                owner.transform.position = data.ManagerRefs.PlayerManager.PlayerBrain.transform.position;            }
        }
    }
}