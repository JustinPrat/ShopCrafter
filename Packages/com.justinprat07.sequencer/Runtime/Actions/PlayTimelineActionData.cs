using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "PlayTimelineActionData", menuName = "Sequencer/PlayTimelineActionData")]
    public class PlayTimelineActionData : SequenceActionData
    {
        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            PlayTimelineActionBehavior behavior = new PlayTimelineActionBehavior();
            behavior.Setup(this, owner);
            return behavior;
        }

        public class PlayTimelineActionBehavior : SequenceActionBehavior
        {
            private PlayTimelineActionData data;
            private PlayableDirector playableDirector;
            private bool isPlaying;

            public void Setup(PlayTimelineActionData data, GameObject owner)
            {
		        this.data = data;
		        this.owner = owner;

                playableDirector = owner.GetComponent<PlayableDirector>();
            }

            public override IEnumerator Execute()
            {
                isPlaying = true;
                playableDirector.Play();
                playableDirector.stopped -= OnPlayableDirectorStopped;
                playableDirector.stopped += OnPlayableDirectorStopped;

                while (isPlaying)
                {
                    yield return null;
                }
            }

            private void OnPlayableDirectorStopped(PlayableDirector director)
            {
                isPlaying = false;
            }

            public override void SetExecuteBaseValue()
            {
            }

            public override void Stop()
            {                playableDirector.Stop();                playableDirector.stopped -= OnPlayableDirectorStopped;            }
        }
    }
}