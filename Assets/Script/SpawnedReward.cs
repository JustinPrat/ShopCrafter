using UnityEngine;

public class SpawnedReward : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Collider col;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    Sequencer.Sequencer sequenceSpawn;

    [SerializeField]
    Sequencer.Sequencer sequenceFly;

    public Rigidbody RB => rb;

    public void Setup(IRewardable rewardable)
    {
        IRewardable.UIDisplayData displayData = rewardable.GetRewardDisplayData();
        spriteRenderer.sprite = displayData.Icon;
    }

    private void OnCollisionEnter(Collision collision)
    {
        FlyToPlayer();
    }

    public void FlyToPlayer()
    {
        col.isTrigger = true;
        rb.isKinematic = true;

        if (sequenceFly != null)
            sequenceFly.StartSequence();
    }
}
