using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "PlaySequencerActionData", menuName = "Sequencer/PlaySequencerActionData")]
    public class PlaySequencerActionData : SequenceActionData
    {
        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            return new PlaySequencerActionBehavior(owner, this);
        }

        public class PlaySequencerActionBehavior : SequenceActionBehavior
        {
            private PlaySequencerActionData data;            private Sequencer sequencer;
            public PlaySequencerActionBehavior(GameObject owner, PlaySequencerActionData data) : base(owner)
            {
                this.data = data;
            }

            public override void Setup()
            {
                sequencer = owner.GetComponent<Sequencer>();
            }

            public override IEnumerator Execute()
            {
                yield return sequencer.StartCoroutine(sequencer.ExecuteSequence());
            }
        }
    }
}