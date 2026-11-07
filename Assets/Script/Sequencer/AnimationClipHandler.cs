using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationClipHandler : MonoBehaviour
{
    [SerializeField] private AnimationClip animationClip;
    [SerializeField] private bool isPlayedAtStart;

    private bool isPlaying = false;
    private float timer = -1f;
    private Animator animator;

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
        if (isPlayedAtStart)
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

            if (animationClip.isLooping)
            {
                PlayAnimation();
            }
        }
    }

    private void OnValidate()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator.enabled)
            animator.enabled = false;
    }
}

