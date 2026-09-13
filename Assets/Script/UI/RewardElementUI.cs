using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardElementUI : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private TextMeshProUGUI nameText;

    public float SpawnedDuration { get; set; }

    public void Setup(IRewardable rewardable)
    {
        IRewardable.UIDisplayData displayData = rewardable.GetRewardDisplayData();
        iconImage.sprite = displayData.Icon;
        nameText.text = displayData.DisplayName;
    }
}
