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
    private TextMeshProUGUI amountWhenDoneText;

    [SerializeField]
    private TextMeshProUGUI amountWinText;

    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private UIEffect uiEffectBackground;

    public void Setup(Objective objective)
    {
        nameText.text = objective.Name;
        descriptionText.text = objective.Description;
        amountWhenDoneText.text = objective.ReputationAmount.ToString();
        amountWinText.text = "";
        iconImage.sprite = objective.Icon;

        if (objective.BGPreset != null)
            uiEffectBackground.LoadPreset(objective.BGPreset);
    }
}