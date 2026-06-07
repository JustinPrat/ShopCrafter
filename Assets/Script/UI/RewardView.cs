using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RewardView : UIView
{
    private const string OutTrigger = "Out";

    [SerializeField] 
    private Image rewardIconImage;

    [SerializeField]
    private TextMeshProUGUI rewardName;

    [SerializeField]
    private Image highlightImage;

    [SerializeField]
    private float outAnimDuration = 0.5f;

    [SerializeField]
    private Animator animator;

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
        }
        else
        {
            animator.SetTrigger(OutTrigger);
            StartCoroutine(WaitForDuration(Deactivate, outAnimDuration));
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
