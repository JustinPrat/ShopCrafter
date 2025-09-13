using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedButton : Selectable, IPointerClickHandler, ISubmitHandler
{
    public UnityEvent OnLeftClick;
    public UnityEvent OnRightClick;

    [SerializeField]
    private ManagerRefs managerRefs;

    private Coroutine _resetRoutine;

    public void OnPointerClick(PointerEventData eventData)
    {
        DoStateTransition(SelectionState.Pressed, true);

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                OnLeftClick?.Invoke();
                break;
            case PointerEventData.InputButton.Right:
                OnRightClick?.Invoke();
                break;
            case PointerEventData.InputButton.Middle:
                break;
            default:
                break;
        }

        if (_resetRoutine != null)
        {
            StopCoroutine(OnFinishSubmit());
        }

        _resetRoutine = StartCoroutine(OnFinishSubmit());
    }

    private IEnumerator OnFinishSubmit ()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }

    protected override void Reset()
    {
        base.Reset();

        var imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            imageComponent = gameObject.AddComponent<Image>();
        }

        targetGraphic = imageComponent;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (managerRefs.InputManager.Actions.UI.Validate.IsPressed())
        {
            OnLeftClick?.Invoke();
        }
        else if (managerRefs.InputManager.Actions.UI.Remove.IsPressed())
        {
            OnRightClick?.Invoke();
        }

        if (_resetRoutine != null)
        {
            StopCoroutine(OnFinishSubmit());
        }

        _resetRoutine = StartCoroutine(OnFinishSubmit());
    }
}
