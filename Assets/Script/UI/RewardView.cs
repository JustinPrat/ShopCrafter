using Coffee.UIEffects;
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
    private Image rewardIconImage;

    [SerializeField]
    private TextMeshProUGUI rewardName;

    [SerializeField]
    private Image highlightImage;

    [SerializeField]
    private Transform toTween;

    [SerializeField]
    private UIEffect uiEffectBG;

    [SerializeField]
    private UIEffectPreset defaultBGUIPreset;

    private bool hasModifiedPreset;

    public void Setup(IRewardable rewardable, UIEffectPreset uiEffectPreset = null)
    {
        IRewardable.UIDisplayData displayData = rewardable.GetRewardDisplayData();
        rewardIconImage.sprite = displayData.Icon;
        rewardName.text = displayData.DisplayName;
        highlightImage.color = displayData.HighlightColor;

        if (uiEffectPreset != null)
        {
            hasModifiedPreset = true;
            uiEffectBG.LoadPreset(uiEffectPreset);
        }
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            managerRefs.InputManager.Actions.UI.Submit.started += OnNextDialogueStarted;
            managerRefs.InputManager.Actions.UI.Cancel.performed += OnCancelPerformed;
        }
        else
        {
            managerRefs.InputManager.Actions.UI.Submit.started -= OnNextDialogueStarted;
            managerRefs.InputManager.Actions.UI.Cancel.performed -= OnCancelPerformed;

            if (hasModifiedPreset)
            {
                uiEffectBG.LoadPreset(defaultBGUIPreset);
            }
        }
    }

    protected override void SetVisualActivationView(bool isOn)
    {
        if (isOn)
        {
            gameObject.SetActive(true);
            Tween.LocalPositionY(toTween, inPositionSettings);
            Tween.Custom(0, 1, inPositionSettings.settings.duration, onValueChange: newVal => canvasGroup.alpha = newVal, useUnscaledTime: true);
        }
        else
        {
            Tween.LocalPositionY(toTween, outPositionSettings).OnComplete(() => Deactivate());
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
