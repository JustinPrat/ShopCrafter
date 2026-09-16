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
    private Sequencer.Sequencer sequenceSpawn;

    [SerializeField]
    private Sequencer.Sequencer touchGroundSequence;

    [SerializeField]
    private LayerMask groundLayer;

    public Rigidbody RB => rb;

    public void Setup(IRewardable rewardable)
    {
        IRewardable.UIDisplayData displayData = rewardable.GetRewardDisplayData();

        if (spriteRenderer != null)
            spriteRenderer.sprite = displayData.Icon;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMethods.IsInLayerMask(collision.gameObject, groundLayer))
        {
            OnTouchGround();
        }
    }

    public void OnTouchGround()
    {
        col.isTrigger = true;
        rb.isKinematic = true;

        if (touchGroundSequence != null)
            touchGroundSequence.StartSequence();
    }
}
