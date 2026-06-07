using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RewardView : UIView
{
    [SerializeField] 
    private TweenSettings<float> inPositionSettings;

    [SerializeField]
    private TweenSettings<float> outPositionSettings;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField] 
    private Image rewardIconImage;

    [SerializeField]
    private TextMeshProUGUI rewardName;

    [SerializeField]
    private Image highlightImage;

    public void Setup(IRewardable rewardable)
    {
        IRewardable.UIDisplayData displayData = rewardable.GetRewardDisplayData();
        rewardIconImage.sprite = displayData.Icon;
        rewardName.text = displayData.DisplayName;
        highlightImage.color = displayData.HighlightColor;
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            managerRefs.InputManager.Actions.Player.NextDialogue.started += OnNextDialogueStarted;
            managerRefs.InputManager.Actions.UI.Cancel.performed += OnCancelPerformed;
        }
        else
        {
            managerRefs.InputManager.Actions.Player.NextDialogue.started -= OnNextDialogueStarted;
            managerRefs.InputManager.Actions.UI.Cancel.performed -= OnCancelPerformed;
        }
    }

    protected override void SetVisualActivationView(bool isOn)
    {
        if (isOn)
        {
            gameObject.SetActive(true);
            Tween.LocalPositionY(transform, inPositionSettings);
            Tween.Custom(0, 1, inPositionSettings.settings.duration, onValueChange: newVal => canvasGroup.alpha = newVal, useUnscaledTime: true);
        }
        else
        {
            Tween.LocalPositionY(transform, outPositionSettings).OnComplete(() => Deactivate());
            Tween.Custom(canvasGroup.alpha, 0, outPositionSettings.settings.duration, onValueChange: newVal => canvasGroup.alpha = newVal, useUnscaledTime: true);
        }
    }

    private void OnCancelPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        managerRefs.UIManager.ToggleRewardView(false);
    }

    private void OnNextDialogueStarted(InputAction.CallbackContext ctx)
    {
        managerRefs.UIManager.ToggleRewardView(false);
    }
}
