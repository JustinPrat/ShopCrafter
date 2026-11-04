using TNRD;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableReward", menuName = "ShopCrafter/SpawnableReward")]
public class SpawnableReward : ScriptableObject, IRewardable
{
    public SerializableInterface<IRewardable> Reward;

    [SerializeField]
    private float force = 1;

    [SerializeField]
    private float upForce = 0.5f;

    [SerializeField]
    private float upOffset = 0.5f;

    public SpawnedReward RewardPrefab { get => Reward.Value.RewardPrefab; set => Reward.Value.RewardPrefab = value; }

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver)
    {
        Spawn(Reward.Value, giver.transform);
    }

    public IRewardable.UIDisplayData GetRewardDisplayData()
    {
        return Reward.Value.GetRewardDisplayData();
    }

    public SpawnedReward Spawn(IRewardable rewardable, Transform transform)
    {
        if (rewardable.RewardPrefab == null)
            return null;

        Vector2 rand = Random.insideUnitCircle;
        Vector3 launchForce = new Vector3(rand.x * force, upForce, rand.y * force);

        SpawnedReward reward = Instantiate(rewardable.RewardPrefab, transform.position + Vector3.up * upOffset, Quaternion.identity);
        reward.RB.AddForce(launchForce, ForceMode.Impulse);
        reward.Setup(rewardable);

        return reward;
    }
}
