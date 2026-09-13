using Sequencer;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    //[SerializeField] 
    //private Transform objectHoldAnchor;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private Interaction interaction;

    [SerializeField]
    private RotateWithDirection rotateWithDirection;

    [SerializeField]
    private Sequencer.Sequencer rewardFeedback;

    [SerializeField]
    private Transform rewardAnchor;

    [SerializeField]
    private RewardElementUI rewardUIPrefab;

    [SerializeField]
    private float rewardUIDuration = 3;

    private Vector2 lastPlayerMovement;
    private Vector2 lastValidDirectionPlayerMovement;

    public Inventory Inventory => inventory;
    public Interaction Interaction => interaction;
    public Vector2 LastPlayerMovement => lastPlayerMovement;
    public Vector2 LastValidDirectionPlayerMovement => lastValidDirectionPlayerMovement;
    public ManagerRefs ManagerRefsProperty => managerRefs;

    private List<RewardElementUI> rewardElementUIs = new List<RewardElementUI>();

    private void Start()
    {
        managerRefs.GameEventsManager.playerEvents.OnPlayerGetRewardFeedback += OnPlayerGetRewardFeedback;
    }

    private void OnDestroy()
    {
        managerRefs.GameEventsManager.playerEvents.OnPlayerGetRewardFeedback -= OnPlayerGetRewardFeedback;
    }

    private void OnPlayerGetRewardFeedback()
    {
        if (rewardFeedback != null)
            rewardFeedback.StartSequence();

        foreach (RewardElementUI rewardElementUI in rewardElementUIs)
        {
            if (!rewardElementUI.gameObject.activeInHierarchy)
            {
                rewardElementUI.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void QueueNextRewardUI(IRewardable rewardable)
    {
        RewardElementUI rewardElementUI = Instantiate(rewardUIPrefab, rewardAnchor);
        rewardElementUI.Setup(rewardable);
        rewardElementUI.gameObject.SetActive(false);
        rewardElementUI.SpawnedDuration = rewardUIDuration;
        rewardElementUIs.Add(rewardElementUI);
    }

    public void SetLastPlayerMovement(Vector2 movement)
    {
        lastPlayerMovement = movement;
        rotateWithDirection.UpdateWithRotation(movement);

        if (movement.magnitude >= 0.1f)
        {
            lastValidDirectionPlayerMovement = movement.normalized;
        }
    }

    public void StopMovementPlayer()
    {
        rotateWithDirection.StopMovement();
    }

    private void Update()
    {
        for (int i = rewardElementUIs.Count - 1; i >= 0; i--)
        {
            RewardElementUI rewardElementUI = rewardElementUIs[i];
            if (rewardElementUI.gameObject.activeInHierarchy)
            {
                rewardElementUI.SpawnedDuration -= Time.deltaTime;

                if (rewardElementUI.SpawnedDuration <= 0)
                {
                    rewardElementUIs.Remove(rewardElementUI);
                    Destroy(rewardElementUI.gameObject);
                }
            }
        }
    }
}
