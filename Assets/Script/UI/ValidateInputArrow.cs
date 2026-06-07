using DG.Tweening;
using System;
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
            transform.DOScale(Vector3.one * 0.8f, 0.2f).SetEase(Ease.InBack).SetUpdate(true);
            slider.DOValue(1, holdDuration).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(() =>
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
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
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
        transform.DOLocalMoveY(moveDistance, moveDuration).SetEase(moveCurve).SetUpdate(true).SetLoops(-1, LoopType.Yoyo);

        if (hold)
        {
            slider.gameObject.SetActive(false);
        }
    }
}
