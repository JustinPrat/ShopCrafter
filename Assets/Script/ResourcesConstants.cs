using UnityEngine;

public class ResourcesConstants
{ 
    private const string coinIcon = "CoinIcon";
    private const string questIcon = "QuestIcon";
    private const string milestoneUpgradeIcon = "MilestoneUpgradeIcon";

    public static Sprite CoinIcon => Resources.Load<Sprite>(coinIcon);
    public static Sprite QuestIcon => Resources.Load<Sprite>(questIcon);
    public static Sprite MilestoneUpgradeIcon => Resources.Load<Sprite>(milestoneUpgradeIcon);
}
