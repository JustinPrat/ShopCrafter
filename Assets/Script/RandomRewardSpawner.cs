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
    private List<Transform> openAnchors = new List<Transform>();
    private List<Transform> closedAnchors = new List<Transform>();


    private void Awake()
    {
        foreach (Transform child in transform)
        {
            openAnchors.Add(child);
        }
    }

    private void Start()
    {
        timer = timeBeforeNewReward;
    }

    private void Update()
    {
        if (enabledRewards.Count < maxRewardNumber && openAnchors.Count > 0)
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
        int childIndex = Random.Range(0, openAnchors.Count);
        PickRewardable pickRewardable = Instantiate(pickPrefab, transform.GetChild(childIndex));
        pickRewardable.SetupReward(rewardablePool.GetRandomElement());
        enabledRewards.Add(pickRewardable);

        closedAnchors.Add(openAnchors[childIndex]);
        openAnchors.RemoveAt(childIndex);

        pickRewardable.OnDestroyEvent += OnPickDestroyed;
    }

    private void OnPickDestroyed(IInteractable interactable)
    {
        PickRewardable pickRewardable = interactable as PickRewardable;

        closedAnchors.Remove(pickRewardable.transform.parent);
        openAnchors.Add(pickRewardable.transform.parent);

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
