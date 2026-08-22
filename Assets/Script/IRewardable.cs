using UnityEngine;

public interface IRewardable
{
    public SpawnedReward RewardPrefab { get; set; }
    public void OnGetReward(ManagerRefs managerRefs, GameObject giver = null);
    public UIDisplayData GetRewardDisplayData();

    public class UIDisplayData
    {
        public string DisplayName;
        public Sprite Icon;
        public Color HighlightColor = Color.white;
    }
}
