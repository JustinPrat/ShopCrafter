using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using PrimeTween;

public class ValidateInputArrow : MonoBehaviour
{
    [SerializeField]
    private float moveDistance = 1f;

    [SerializeField]
    private float moveDuration = 1f;

    [SerializeField]
    private Sprite controllerIcon;

    [SerializeField] 
    private Sprite keyboardIcon;

    [SerializeField]
    private Image iconHolder;

    [SerializeField]
    private bool hold = false;

    [SerializeField]
    private float holdDuration = 1f;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private AnimationCurve moveCurve;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private bool onEnabledMove = true;
   

    private void Awake()
    {
        if (!hold)
        {
            slider.gameObject.SetActive(false);
        }

        managerRefs.InputManager.Actions.Player.NextDialogue.performed += OnNextDialoguePressed;
        managerRefs.InputManager.OnInputDeviceChanged += OnInputDeviceChanged;

        OnInputDeviceChanged();
    }

    private void OnEnable()
    {
        if (onEnabledMove)
        {
            StartMoveArrow();
        }
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.Player.NextDialogue.performed -= OnNextDialoguePressed;
        managerRefs.InputManager.OnInputDeviceChanged -= OnInputDeviceChanged;
    }

    private void OnInputDeviceChanged()
    {
        iconHolder.sprite = managerRefs.InputManager.IsGamepad ? controllerIcon : keyboardIcon;
    }

    private void OnNextDialoguePressed(InputAction.CallbackContext context)
    {
        if (hold)
        {
            slider.gameObject.SetActive(true);

            Tween.Scale(transform, 0.8f, 0.2f, ease: Ease.InBack, useUnscaledTime: true);
            Tween.UISliderValue(slider, slider.value, 1, holdDuration, ease: Ease.InOutSine, useUnscaledTime: true).OnComplete(() =>
            {
                slider.value = 0;
                OnPressValidation();
            });
        }
        else
        {
            OnPressValidation();
        }
    }

    private void OnPressValidation()
    {
        Tween.Scale(transform, 1f, 0.2f, ease: Ease.OutBack, useUnscaledTime: true).OnComplete(() =>
        {
            transform.localScale = Vector3.one;
            StartMoveArrow();
        });

        if (hold)
        {
            slider.gameObject.SetActive(false);
        }
    }

    public void StartMoveArrow()
    {
        if (!transform.gameObject.activeInHierarchy)
            return;

        transform.localPosition = Vector3.zero;
        Tween.LocalPositionY(transform, moveDistance, moveDuration, ease: moveCurve, cycles: -1, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);

        if (hold)
        {
            slider.gameObject.SetActive(false);
        }
    }
}
