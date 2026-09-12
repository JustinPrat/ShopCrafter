using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "PlayerGetRewardActionData", menuName = "Sequencer/PlayerGetRewardActionData")]
    public class PlayerGetRewardActionData : SequenceActionData
    {
        public ManagerRefs ManagerRefs;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            PlayerGetRewardActionBehavior behavior = new PlayerGetRewardActionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class PlayerGetRewardActionBehavior : SequenceActionBehavior
        {
            private PlayerGetRewardActionData data;

            public void Setup(PlayerGetRewardActionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;
            }

            public override IEnumerator Execute()
            {
                data.ManagerRefs.GameEventsManager.playerEvents.PlayerGetRewardFeedback();
                yield return null;
            }

            public override void SetExecuteBaseValue()
            {
            }

            public override void Stop()
            {            }
        }
    }
}