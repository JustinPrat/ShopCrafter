using System;
using System.Collections;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class AdvancedButton : Selectable, IPointerClickHandler, ISubmitHandler
{
    public UnityEvent<AdvancedButton> OnLeftClick;
    public UnityEvent<AdvancedButton> OnRightClick;

    public UnityEvent<AdvancedButton> OnLeftClickReleased;

    public Action<AdvancedButton> OnLeftClickUp;
    public Action<AdvancedButton> OnLeftClickDown;

    public Action<AdvancedButton> OnSelected;
    public Action<AdvancedButton> OnDeselected;

    public UnityEvent OnTransitionNormal;
    public UnityEvent OnTransitionHighlighted;
    public UnityEvent OnTransitionPressed;
    public UnityEvent OnTransitionSelected;
    public UnityEvent OnTransitionDisabled;
    public ButtonSelectionState CurrentState => currentState;
    public bool IsPointerInside => isPointerInside;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private bool toggleDebug;

    [Serializable]
    public enum ButtonSelectionState
    {
        Normal = 0,
        Highlighted = 1,
        Pressed = 2,
        Selected = 3,
        Disabled = 4
    }

    private static AdvancedButton lastPointerButton;

    private Coroutine resetRoutine;
    private bool isPointerInside;
    private bool isSubmitHeld;
    private float submitPressTime;
    private InputAction activeSubmitAction;
    private InputReceiver boundInputManager;

    private bool IsGamepad => managerRefs != null && managerRefs.InputManager != null && managerRefs.InputManager.IsGamepad;

    [SerializeField, ReadOnly]
    private ButtonSelectionState currentState = ButtonSelectionState.Normal;

    public void Click(PointerEventData.InputButton button)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current)
        {
            button = button
        };

        OnPointerClick(pointer);
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (isSubmitHeld && state != SelectionState.Pressed && state != SelectionState.Disabled)
            return;

        if (!IsGamepad && state == SelectionState.Selected)
        {
            state = isPointerInside ? SelectionState.Highlighted : SelectionState.Normal;
        }

        base.DoStateTransition(state, instant);
        currentState = (ButtonSelectionState)state;

        switch (state)
        {
            case SelectionState.Normal:
                OnTransitionNormal?.Invoke();
                break;
            case SelectionState.Highlighted:
                OnTransitionHighlighted?.Invoke();
                break;
            case SelectionState.Pressed:
                OnTransitionPressed?.Invoke();
                break;
            case SelectionState.Selected:
                OnTransitionSelected?.Invoke();
                break;
            case SelectionState.Disabled:
                OnTransitionDisabled?.Invoke();
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable)
            return;

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                OnLeftClick?.Invoke(this);
                if (toggleDebug)
                    Debug.Log("Left Click On Button");
                break;

            case PointerEventData.InputButton.Right:
                OnRightClick?.Invoke(this);
                if (toggleDebug)
                    Debug.Log("Right Click On Button");
                break;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        BindInputManager();
        isPointerInside = true;
        lastPointerButton = this;
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        base.OnPointerExit(eventData);

        if (!IsGamepad)
            ClearEventSystemSelection();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        BindInputManager();
        lastPointerButton = this;

        if (eventData.button == PointerEventData.InputButton.Left && IsInteractable())
        {
            DoStateTransition(SelectionState.Pressed, false);
            OnLeftClickDown?.Invoke(this);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!IsGamepad)
                ClearEventSystemSelection();

            if (IsInteractable())
            {
                DoStateTransition(isPointerInside ? SelectionState.Highlighted : SelectionState.Normal, false);
            }

            OnLeftClickUp?.Invoke(this);
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        OnDeselected?.Invoke(this);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        if (IsGamepad)
            OnSelected?.Invoke(this);
    }

    private void ClearEventSystemSelection()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null && !eventSystem.alreadySelecting && eventSystem.currentSelectedGameObject == gameObject)
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }

    private void HandleInputDeviceChanged()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null || eventSystem.alreadySelecting)
            return;

        if (!IsGamepad)
        {
            if (eventSystem.currentSelectedGameObject == gameObject)
            {
                lastPointerButton = this;
                eventSystem.SetSelectedGameObject(null);
            }
        }
        else if (eventSystem.currentSelectedGameObject == null && lastPointerButton == this && IsInteractable())
        {
            eventSystem.SetSelectedGameObject(gameObject);
        }
    }

    private void BindInputManager()
    {
        InputReceiver currentManager = managerRefs != null ? managerRefs.InputManager : null;
        if (boundInputManager == currentManager)
            return;

        UnbindInputManager();

        if (currentManager != null)
        {
            boundInputManager = currentManager;
            boundInputManager.OnInputDeviceChanged += HandleInputDeviceChanged;
        }
    }

    private void UnbindInputManager()
    {
        if (boundInputManager != null)
        {
            boundInputManager.OnInputDeviceChanged -= HandleInputDeviceChanged;
            boundInputManager = null;
        }
    }

    private InputAction GetSubmitAction()
    {
        if (EventSystem.current != null &&
            EventSystem.current.currentInputModule is InputSystemUIInputModule uiModule &&
            uiModule.submit != null)
        {
            return uiModule.submit.action;
        }

        return managerRefs != null && managerRefs.InputManager != null
            ? managerRefs.InputManager.Actions.UI.Submit
            : null;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (!interactable)
            return;

        isSubmitHeld = true;
        submitPressTime = Time.unscaledTime;

        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }

        UnsubscribeSubmitCanceled();
        activeSubmitAction = GetSubmitAction();
        if (activeSubmitAction != null)
        {
            activeSubmitAction.canceled += OnSubmitCanceled;
        }

        DoStateTransition(SelectionState.Pressed, true);
        OnLeftClick?.Invoke(this);

        if (toggleDebug)
            Debug.Log("Submit Left");
    }

    private void OnSubmitCanceled(InputAction.CallbackContext context)
    {
        UnsubscribeSubmitCanceled();

        if (!isSubmitHeld)
            return;

        isSubmitHeld = false;

        if (this == null || !interactable)
            return;

        OnLeftClickReleased?.Invoke(this);

        SelectionState targetState = GetFallbackState();
        float remainingTime = colors.fadeDuration - (Time.unscaledTime - submitPressTime);

        if (remainingTime > 0f && gameObject.activeInHierarchy)
        {
            RestartResetRoutine(targetState, remainingTime);
        }
        else
        {
            DoStateTransition(targetState, false);
        }

        if (toggleDebug)
            Debug.Log("Submit Left Out");
    }

    private SelectionState GetFallbackState()
    {
        bool isStillSelected = IsGamepad &&
                               EventSystem.current != null &&
                               EventSystem.current.currentSelectedGameObject == gameObject;

        return isStillSelected
            ? SelectionState.Selected
            : (isPointerInside ? SelectionState.Highlighted : SelectionState.Normal);
    }

    private void RestartResetRoutine(SelectionState targetState, float delay)
    {
        if (this == null || !gameObject.activeInHierarchy)
            return;

        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }

        resetRoutine = StartCoroutine(OnFinishSubmit(targetState, delay));
    }

    private IEnumerator OnFinishSubmit(SelectionState targetState, float delay)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delay)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (IsInteractable())
        {
            DoStateTransition(targetState, false);
        }

        resetRoutine = null;
    }

    private void UnsubscribeSubmitCanceled()
    {
        if (activeSubmitAction != null)
        {
            activeSubmitAction.canceled -= OnSubmitCanceled;
            activeSubmitAction = null;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        BindInputManager();
        isPointerInside = false;
        isSubmitHeld = false;
        DoStateTransition(IsInteractable() ? SelectionState.Normal : SelectionState.Disabled, true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UnbindInputManager();
        UnsubscribeSubmitCanceled();

        if (lastPointerButton == this)
            lastPointerButton = null;

        isPointerInside = false;
        isSubmitHeld = false;

        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }
    }

#if UNITY_EDITOR
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
#endif
}