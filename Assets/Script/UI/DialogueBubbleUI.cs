using DG.Tweening;
using TMPEffects.Components;
using UnityEngine;

public class DialogueBubbleUI : MonoBehaviour
{
    [SerializeField]
    private TMPAnimator textAnimator;

    [SerializeField]
    private TMPWriter textWriter;

    [SerializeField]
    private RectTransform mainRectTransform;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private GameObject clickToSkip;

    private float bubbleHeight;

    public TMPWriter TextWriter => textWriter;
    public float BubbleHeight => bubbleHeight;
    public RectTransform MainRectTransform => mainRectTransform;

    public void SetText(string newText)
    {
        textAnimator.SetText(newText);
        UpdateBubbleHeight();
    }

    public void SetClickVisual(bool isActive)
    {
        clickToSkip.SetActive(isActive);
    }

    public void UpdateBubbleHeight()
    {
        bubbleHeight = mainRectTransform.rect.height;
    }

    public void SetTransparency(float alpha)
    {
        canvasGroup.alpha = alpha;
    }

    public void SetTransparency(float alpha, float duration)
    {
        canvasGroup.DOFade(alpha, duration).SetUpdate(true);
    }

    private void Awake()
    {
        if (mainRectTransform == null)
        {
            mainRectTransform = GetComponent<RectTransform>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        UpdateBubbleHeight();

        clickToSkip.SetActive(false);
    }
}
