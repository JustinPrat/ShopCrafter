using PrimeTween;
using TMPEffects.Components;
using UnityEngine;

public class DialogueBubbleUI : MonoBehaviour
{
    [SerializeField]
    private TMPAnimator textAnimator;

    [SerializeField]
    private TMPWriter textWriter;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private GameObject clickToSkip;

    public TMPWriter TextWriter => textWriter;

    public bool IsWriting => textWriter.IsWriting;

    public void SetText(string newText)
    {
        textAnimator.SetText(newText);
        
        if (textAnimator.TextComponent.textInfo.lineCount <= 1)
        {
            textAnimator.TextComponent.alignment = TMPro.TextAlignmentOptions.Center;
        }
        else
        {
            textAnimator.TextComponent.alignment = TMPro.TextAlignmentOptions.Left;
        }
    }

    public void Skip()
    {
        textWriter.SkipWriter();
    }

    public void SetClickVisual(bool isActive)
    {
        clickToSkip.SetActive(isActive);
    }

    public void SetTransparency(float alpha)
    {
        canvasGroup.alpha = alpha;
    }

    public void SetTransparency(float alpha, float duration)
    {
        Tween.Custom(canvasGroup.alpha, alpha, duration, onValueChange: newVal => canvasGroup.alpha = newVal, useUnscaledTime: true);
    }

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        clickToSkip.SetActive(false);
    }
}
