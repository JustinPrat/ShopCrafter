using PrimeTween;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private AnimationCurve moveCurve;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private bool onEnabledMove = true;

    [SerializeField]
    private bool isVertical = true;

    [SerializeField]
    private bool hold = false;

    [SerializeField, ShowIf(nameof(hold))]
    private float holdDuration = 1f;

    [SerializeField, ShowIf(nameof(hold))]
    private Slider slider;

    private Tween currentInfiniteTween;
    private float baseLocalMovingAxis;


    private void Awake()
    {
        if (!hold && slider != null)
        {
            slider.gameObject.SetActive(false);
        }

        managerRefs.InputManager.Actions.UI.Submit.performed += OnSubmitPressed;
        managerRefs.InputManager.OnInputDeviceChanged += OnInputDeviceChanged;

        OnInputDeviceChanged();

        if (isVertical)
            baseLocalMovingAxis = transform.localPosition.y;
        else
            baseLocalMovingAxis = transform.localPosition.x;
    }

    private void OnEnable()
    {
        if (onEnabledMove)
        {
            StartMoveArrow();
        }
    }

    private void OnDisable()
    {
        currentInfiniteTween.Stop();
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.UI.Submit.performed -= OnSubmitPressed;
        managerRefs.InputManager.OnInputDeviceChanged -= OnInputDeviceChanged;
    }

    private void OnInputDeviceChanged()
    {
        iconHolder.sprite = managerRefs.InputManager.IsGamepad ? controllerIcon : keyboardIcon;
    }

    private void OnSubmitPressed(InputAction.CallbackContext context)
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
        StopMoveArrow();

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

        StopMoveArrow();

        if (isVertical)
            currentInfiniteTween = Tween.LocalPositionY(transform, moveDistance, moveDuration, ease: moveCurve, cycles: -1, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);
        else
            currentInfiniteTween = Tween.LocalPositionX(transform, moveDistance, moveDuration, ease: moveCurve, cycles: -1, cycleMode: CycleMode.Yoyo, useUnscaledTime: true);

        if (hold)
        {
            slider.gameObject.SetActive(false);
        }
    }

    public void StopMoveArrow()
    {
        currentInfiniteTween.Stop();

        if (isVertical)
            transform.localPosition = new Vector3(transform.localPosition.x, baseLocalMovingAxis, transform.localPosition.z);
        else
            transform.localPosition = new Vector3(baseLocalMovingAxis, transform.localPosition.y, transform.localPosition.z);
    }
}
