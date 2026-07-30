using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "PlaySoundActionData", menuName = "Sequencer/PlaySoundActionData")]
    public class PlaySoundActionData : SequenceActionData
    {
        public AudioClip AudioClip;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            return new PlaySoundActionBehavior(owner, this);
        }

        public class PlaySoundActionBehavior : SequenceActionBehavior
        {
            private PlaySoundActionData data;
            public PlaySoundActionBehavior(GameObject owner, PlaySoundActionData data) : base(owner)
            {
                this.data = data;
            }

            public override void Setup()
            {
            }

            public override IEnumerator Execute()
            {
                GameObject gameObject = new GameObject();
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = data.AudioClip;
                audioSource.Play();

                Destroy(gameObject, data.AudioClip.length);
                yield return null;
            }
        }
    }
}