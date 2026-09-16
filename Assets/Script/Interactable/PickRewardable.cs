using System;
using System.Collections.Generic;
using TNRD;
using Unity.Cinemachine;
using UnityEngine;

public class PickRewardable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private List<SerializableInterface<IRewardable>> rewardsSerialized;

    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private CinemachineImpulseSource cinemachineImpulseSource;

    [SerializeField]
    private RewardSpawner rewardSpawner;

    private List<IRewardable> rewards = new List<IRewardable>();

    public bool IsLocked { get; set; }

    public string InteractText => "Take";

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }

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

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public void DoInteract(PlayerBrain playerBrain)
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

        cinemachineImpulseSource.GenerateImpulse(0.2f);
        Destroy(gameObject);
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
        OnTargetedEvent?.Invoke(this);
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
        OnUnTargetedEvent?.Invoke(this);
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }
}
