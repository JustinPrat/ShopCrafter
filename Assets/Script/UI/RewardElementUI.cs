using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardElementUI : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private UIEffect uiEffectBG;

    public float SpawnedDuration { get; set; }

    public void Setup(IRewardable rewardable)
    {
        IRewardable.UIDisplayData displayData = rewardable.GetRewardDisplayData();

        if (iconImage != null)
            iconImage.sprite = displayData.Icon;

        if (nameText != null)
            nameText.text = displayData.DisplayName;
    }

    public void ChangePreset(UIEffectPreset preset)
    {
        if (uiEffectBG != null)
            uiEffectBG.LoadPreset(preset);
    }
}
