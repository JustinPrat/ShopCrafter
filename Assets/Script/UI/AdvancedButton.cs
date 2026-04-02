using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedButton : Selectable, IPointerClickHandler, ISubmitHandler
{
    public UnityEvent<AdvancedButton> OnLeftClick;
    public UnityEvent<AdvancedButton> OnRightClick;

    public UnityEvent<AdvancedButton> OnLeftClickReleased;

    public Action<AdvancedButton> OnSelected;
    public Action<AdvancedButton> OnDeselected;

    [SerializeField]
    private ManagerRefs managerRefs;

    private Coroutine _resetRoutine;

    [SerializeField]
    private bool toggleDebug;

    private bool buttonPressed = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        DoStateTransition(SelectionState.Pressed, true);

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                OnLeftClick?.Invoke(this);
                buttonPressed = true;
                if (toggleDebug)
                {
                    Debug.Log("Left Click On Button");
                }
                break;
            case PointerEventData.InputButton.Right:
                OnRightClick?.Invoke(this);
                if (toggleDebug)
                {
                    Debug.Log("Right Click On Button");
                }
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

        if (gameObject.activeInHierarchy)
        {
            _resetRoutine = StartCoroutine(OnFinishSubmit());
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        switch (state)
        {
            case SelectionState.Normal:
                if (toggleDebug)
                {
                    Debug.Log("Normal");
                }
                break;
            case SelectionState.Highlighted:
                if (toggleDebug)
                {
                    Debug.Log("Highlighted");
                }
                break;
            case SelectionState.Pressed:
                if (toggleDebug)
                {
                    Debug.Log("Pressed");
                }
                break;
            case SelectionState.Selected:
                if (toggleDebug)
                {
                    Debug.Log("Selected");
                }
                break;
            case SelectionState.Disabled:
                if (toggleDebug)
                {
                    Debug.Log("Disabled");
                }
                break;
        }
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

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        OnDeselected?.Invoke(this);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        OnSelected?.Invoke(this);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (toggleDebug)
        {
            Debug.Log("Out Click On Button");
        }
    }

    private void Update()
    {
        if (buttonPressed && !managerRefs.InputManager.Actions.UI.Validate.IsPressed())
        {
            DoStateTransition(SelectionState.Selected, true);
            OnLeftClickReleased?.Invoke(this);
            buttonPressed = false;

            if (toggleDebug)
            {
                Debug.Log("Submit Left Out");
            }
        }
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
            DoStateTransition(SelectionState.Pressed, true);
            OnLeftClick?.Invoke(this);
            buttonPressed = true;
            if (toggleDebug)
            {
                Debug.Log("Submit Left");
            }
        }
        else if (managerRefs.InputManager.Actions.UI.Remove.IsPressed())
        {
            DoStateTransition(SelectionState.Pressed, true);
            OnRightClick?.Invoke(this);
            if (toggleDebug)
            {
                Debug.Log("Submit Right");
            }
        }

        if (_resetRoutine != null)
        {
            StopCoroutine(OnFinishSubmit());
        }

        if (gameObject.activeInHierarchy)
        {
            _resetRoutine = StartCoroutine(OnFinishSubmit());
        }
    }
}
