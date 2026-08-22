using UnityEngine;

public class RewardSpawner : MonoBehaviour
{
    [SerializeField]
    private float force = 1;

    [SerializeField]
    private float upForce = 0.5f;

    [SerializeField]
    private float upOffset = 0.5f;

    public void Spawn(IRewardable rewardable)
    {
        if (rewardable.RewardPrefab == null)
            return;

        Vector2 rand = Random.insideUnitCircle;
        Vector3 launchForce = new Vector3(rand.x * force, upForce, rand.y * force);

        SpawnedReward reward = Instantiate(rewardable.RewardPrefab, transform.position + Vector3.up * upOffset, Quaternion.identity);
        reward.RB.AddForce(launchForce, ForceMode.Impulse);
        reward.Setup(rewardable);
    }
}
