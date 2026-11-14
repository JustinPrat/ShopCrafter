using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI descriptionText;

    [SerializeField]
    private TextMeshProUGUI amountRewardText;

    [SerializeField]
    private TextMeshProUGUI currentAmountText;

    [SerializeField]
    private TextMeshProUGUI amountMaxText;

    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private UIEffect uiEffectBackground;
    private ObjectiveBehavior objectiveBehavior;

    public void Setup(ObjectiveBehavior objective)
    {
        nameText.text = objective.Data.Name;
        descriptionText.text = objective.Data.Description;
        amountRewardText.text = objective.Data.ReputationAmount.ToString();
        amountMaxText.text = objective.Data.ReputationMax.ToString();
        currentAmountText.text = objective.ReputationGained.ToString();

        iconImage.sprite = objective.Data.Icon;

        if (objective.Data.BGPreset != null)
            uiEffectBackground.LoadPreset(objective.Data.BGPreset);

        objectiveBehavior = objective;
        objectiveBehavior.OnReputationUpdate += OnReputationUpdate;
    }

    private void OnDestroy()
    {
        if (objectiveBehavior != null)
            objectiveBehavior.OnReputationUpdate -= OnReputationUpdate;
    }

    private void OnReputationUpdate(ObjectiveBehavior objective)
    {
        currentAmountText.text = objective.ReputationGained.ToString();
    }
}