using UnityEngine;
using UnityEngine.UI;

public class TagPlaceholderUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private LayoutElement layoutElement;

    public void Setup(RectTransform toCopy)
    {
        rectTransform.sizeDelta = toCopy.sizeDelta;
        layoutElement.preferredWidth = toCopy.rect.width;
        layoutElement.preferredHeight = toCopy.rect.height;
    }
}
