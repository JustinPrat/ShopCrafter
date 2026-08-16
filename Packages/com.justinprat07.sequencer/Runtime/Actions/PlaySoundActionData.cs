using System.Collections;
using UnityEngine;
using static Sequencer.Actions.ActivationActionData;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "PlaySoundActionData", menuName = "Sequencer/PlaySoundActionData")]
    public class PlaySoundActionData : SequenceActionData
    {
        public AudioClip AudioClip;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            PlaySoundActionBehavior playSoundActionBehavior = new PlaySoundActionBehavior();
            playSoundActionBehavior.Setup(this, owner);
            return playSoundActionBehavior;
        }

        public class PlaySoundActionBehavior : SequenceActionBehavior
        {
            private PlaySoundActionData data;

            public void Setup(PlaySoundActionData data, GameObject owner)
            {
                this.data = data;
                this.owner = owner;
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

            public override void Stop()
            {
            }

            public override void SetExecuteBaseValue()
            {
            }
        }
    }
}