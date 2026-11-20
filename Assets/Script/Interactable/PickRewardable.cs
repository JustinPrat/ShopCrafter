using System.Collections.Generic;
using TNRD;
using Unity.Cinemachine;
using UnityEngine;

public class PickRewardable : BaseWorldInteract
{
    [SerializeField]
    private List<SerializableInterface<IRewardable>> rewardsSerialized;

    [SerializeField]
    private CinemachineImpulseSource cinemachineImpulseSource;

    [SerializeField]
    private RewardSpawner rewardSpawner;

    private List<IRewardable> rewards = new List<IRewardable>();

    private void Awake()
    {
        foreach (SerializableInterface<IRewardable> rewardSerialized in rewardsSerialized)
        {
            if (rewardSerialized != null && rewardSerialized.Value != null)
            {
                rewards.Add(rewardSerialized.Value);
            }
        }
    }

    public void SetupReward(SerializableInterface<IRewardable> rewardable)
    {
        rewards.Add(rewardable.Value);
    }

    public void SetupReward(IRewardable rewardable)
    {
        rewards.Add(rewardable);
    }

    public override void StartInteract(PlayerBrain playerBrain)
    {
        if (rewards.Count <= 0)
            return;

        for (int i = 0; i < rewards.Count; i++)
        {
            IRewardable reward = rewards[i];

            IRewardable.UIDisplayData displayData = reward.GetRewardDisplayData();
            if (displayData != null && displayData.DisplayAbovePlayer)
                playerBrain.QueueNextRewardUI(reward);

            reward.OnGetReward(managerRefs, gameObject);
            rewardSpawner.Spawn(reward);
        }

        base.StartInteract(playerBrain);
        cinemachineImpulseSource.GenerateImpulse(0.2f);
        Destroy(gameObject);
    }
}
