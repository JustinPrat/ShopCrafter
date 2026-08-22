using System;
using TNRD;
using Unity.Cinemachine;
using UnityEngine;

public class PickRewardable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private SerializableInterface<IRewardable> reward;

    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private CinemachineImpulseSource cinemachineImpulseSource;

    [SerializeField]
    private RewardSpawner rewardSpawner;

    public bool IsLocked { get; set; }

    public string InteractText => "Take";

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }

    public void SetupReward(SerializableInterface<IRewardable> rewardable)
    {
        reward = rewardable;
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        if (reward.Value == null)
            return;
        
        reward.Value.OnGetReward(managerRefs, gameObject);
        cinemachineImpulseSource.GenerateImpulse(0.2f);
        rewardSpawner.Spawn(reward.Value);
        Destroy(gameObject);
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
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
