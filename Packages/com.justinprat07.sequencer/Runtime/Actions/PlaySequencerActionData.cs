using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "PlaySequencerActionData", menuName = "Sequencer/PlaySequencerActionData")]
    public class PlaySequencerActionData : SequenceActionData
    {
        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            PlaySequencerActionBehavior playSequencerActionBehavior = new PlaySequencerActionBehavior();
            playSequencerActionBehavior.Setup(this, owner);
            return playSequencerActionBehavior;
        }

        public class PlaySequencerActionBehavior : SequenceActionBehavior
        {
            private PlaySequencerActionData data;            private Sequencer sequencer;

            public void Setup(PlaySequencerActionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;
            }

            public override IEnumerator Execute()
            {
                yield return sequencer.StartCoroutine(sequencer.ExecuteSequence());
            }

            public override void Stop()
            {
                sequencer.StopSequence();
            }
        }
    }
}