using System;
using UnityEngine;

public class AnimationClipHandler : MonoBehaviour
{
    [SerializeField] private AnimationClip animationClip;
    [SerializeField] private Sequencer.PlayMode playMode;

    private bool isPlaying = false;
    private float timer = -1f;
    private Animator animator;
    private Animation animation;

    public event Action OnClipEnded;
    public bool IsComplete => timer >= animationClip.length;

    public void PlayAnimation()
    {
        if (animationClip == null)
            return;

        timer = 0f;
        isPlaying = true;
    }

    public void PlayAnimation(AnimationClip clip)
    {
        animationClip = clip;
        PlayAnimation();
    }

    private void Start()
    {
        if (playMode == Sequencer.PlayMode.Start)
        {
            PlayAnimation();
        }
    }

    private void OnEnable()
    {
        if (playMode == Sequencer.PlayMode.Enable)
        {
            PlayAnimation();
        }
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        timer += Time.deltaTime;
        animationClip.SampleAnimation(gameObject, timer);

        if (IsComplete)
        {
            OnClipEnded?.Invoke();
            isPlaying = false;

            if ((animationClip.wrapMode == WrapMode.Loop) || (animationClip.wrapMode == WrapMode.PingPong))
            {
                PlayAnimation();
            }
        }
    }

    private void OnValidate()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animation == null)
            animation = GetComponent<Animation>();

        if (animator != null && animator.enabled)
            animator.enabled = false;
    }
}

