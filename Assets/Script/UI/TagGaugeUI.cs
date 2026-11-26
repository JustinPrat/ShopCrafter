using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TagGaugeUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI tagName;

    [SerializeField]
    private Image tagBackground;

    [SerializeField]
    private Image tagIcon;

    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private Image surplusBackground;

    [SerializeField]
    private Color surplusOn;

    [SerializeField]
    private Color surplusOff;

    [SerializeField]
    private TextMeshProUGUI surplusText;

    [SerializeField]
    private Sequencer.Sequencer selectOnSequencer;

    [SerializeField]
    private Sequencer.Sequencer selectOffSequencer;

    [SerializeField]
    private RectTransform errorRect;

    private TagValue currentTag;

    public RectTransform Rect => rectTransform;
    public TagValue CurrentTag => currentTag;

    public void Setup(TagValue tagValue)
    {
        currentTag = tagValue;
        tagName.text = tagValue.Asset.Name;
        tagBackground.color = tagValue.Asset.TagOffGaugeColor;
        tagIcon.sprite = tagValue.Asset.Icon;

        if (tagValue.Surplus > 0)
        {
            gameObject.SetActive(true);
            surplusText.text = "+" + tagValue.Surplus;
            surplusBackground.color = surplusOff;
        }
    }

    public void UpdateValue()
    {
        int surplus = Mathf.Max(currentTag.Amount.Value - currentTag.GaugeValue, 0);
        surplusText.text = "+" + surplus;

        int difference = currentTag.Amount.Value - currentTag.GaugeValue;
        if (difference >= 0)
        {
            // no error
        }
        else
        {
            // error
            errorRect.gameObject.SetActive(true);
            float sizeRatio = (float)Mathf.Abs(difference) / currentTag.GaugeValue;
            float pos = 1 - (sizeRatio/2);
            SetSizePosition(errorRect, rectTransform, pos, sizeRatio);
        }
    }

    public void SetSizePosition(RectTransform rect, RectTransform container, float pos, float size)
    {
        float width = size * container.rect.width;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        float localX = Mathf.LerpUnclamped(container.rect.xMin, container.rect.xMax, pos);
        Vector3 worldPos = container.TransformPoint(new Vector3(localX, container.rect.center.y, 0f));
        rect.position = worldPos;

        rect.SetSiblingIndex(rect.GetSiblingIndex() - 1);
    }

    public void SetSelection(bool selection)
    {
        if (selection)
        {
            if (selectOffSequencer != null)
                selectOffSequencer.StopSequence();

            if (selectOnSequencer != null)
                selectOnSequencer.StartSequence();

            tagBackground.color = currentTag.Asset.TagOnGaugeColor;
            surplusBackground.color = surplusOn;
        }
        else
        {
            if (selectOnSequencer != null)
                selectOnSequencer.StopSequence();

            if (selectOffSequencer != null)
                selectOffSequencer.StartSequence();

            tagBackground.color = currentTag.Asset.TagOffGaugeColor;
            surplusBackground.color = surplusOff;
        }
    }
}
