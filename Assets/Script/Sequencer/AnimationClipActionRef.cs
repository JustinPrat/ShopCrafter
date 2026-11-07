using System;
using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [Serializable]
    public class AnimationClipActionRef : SequenceRefActionData
    {
        public AnimationClipHandler AnimationClipHandlerRef;
        public AnimationClip AnimationClipRef;

        private bool clipPlaying;

        public override IEnumerator Execute()
        {
            clipPlaying = true;
            AnimationClipHandlerRef.PlayAnimation(AnimationClipRef);

            AnimationClipHandlerRef.OnClipEnded -= AnimationClipHandlerRef_OnClipEnded;
            AnimationClipHandlerRef.OnClipEnded += AnimationClipHandlerRef_OnClipEnded;

            while (clipPlaying)
            {
                yield return null;
            }
        }

        private void AnimationClipHandlerRef_OnClipEnded()
        {
            clipPlaying = false;
        }

        public override void SetExecuteBaseValue()
        {
        }

        public override void Stop()
        {
            AnimationClipHandlerRef.OnClipEnded -= AnimationClipHandlerRef_OnClipEnded;
        }
    }
}
