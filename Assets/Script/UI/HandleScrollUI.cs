using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleScrollUI : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scrollRect;

    private GameObject lastSelected;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (lastSelected != EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(scrollRect.transform))
            {
                scrollRect.content.localPosition = scrollRect.GetSnapToPositionToBringChildIntoView(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
            }

            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
    }
}
