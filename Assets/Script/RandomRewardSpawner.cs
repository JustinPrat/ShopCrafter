using System.Collections.Generic;
using TNRD;
using UnityEngine;

public class RandomRewardSpawner : MonoBehaviour
{
    [SerializeField]
    private List<SerializableInterface<IRewardable>> rewardablePool;

    [SerializeField]
    private int maxRewardNumber = 3;

    [SerializeField]
    private float timeBeforeNewReward = 10;

    [SerializeField]
    private PickRewardable pickPrefab;

    private List<PickRewardable> enabledRewards = new List<PickRewardable>();
    private float timer;


    private void Start()
    {
        timer = timeBeforeNewReward;
    }

    private void Update()
    {
        if (enabledRewards.Count < maxRewardNumber)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timeBeforeNewReward;
                Spawn();
            }
        }
    }

    public void Spawn()
    {
        int childIndex = Random.Range(0, transform.childCount);
        PickRewardable pickRewardable = Instantiate(pickPrefab, transform.GetChild(childIndex));
        pickRewardable.SetupReward(rewardablePool.GetRandomElement());
        enabledRewards.Add(pickRewardable);
        pickRewardable.OnDestroyEvent += OnPickDestroyed;
    }

    private void OnPickDestroyed(IInteractable interactable)
    {
        PickRewardable pickRewardable = interactable as PickRewardable;
        enabledRewards.Remove(pickRewardable);
        pickRewardable.OnDestroyEvent -= OnPickDestroyed;
    }

    private void OnDestroy()
    {
        foreach (PickRewardable pickRewardable in enabledRewards)
        {
            pickRewardable.OnDestroyEvent -= OnPickDestroyed;
        }
    }
}
