using System;
using System.Collections;
using UnityEngine;

public class PickCraftedObject : MonoBehaviour, IInteractable, IRewardable
{
    [SerializeField]
    private CraftedObjectRecipe craftedObjectRecipe;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private Sequencer.Sequencer onPickSequencer;

    [SerializeField]
    private SpawnedReward rewardPrefab;

    public bool IsLocked { get; set; }

    public string InteractText => "Take";

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public SpawnedReward RewardPrefab { get => rewardPrefab; set => rewardPrefab = value; }
    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        if (playerBrain.Inventory.HasEmptySpace())
            return true;

        return false;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        if (craftedObjectRecipe != null)
        {
            bool isNew = managerRefs.CraftingManager.IsNew(craftedObjectRecipe);
            CraftedObject craftedObject = Instantiate(managerRefs.CraftingManager.CraftedObjectPrefab);
            CraftedObjectData craftedObjectData = managerRefs.CraftingManager.GetCraftedData(craftedObjectRecipe, isNew);
            craftedObject.Init(craftedObjectData);

            if (playerBrain.Inventory.TryTakeItem(craftedObject))
            {
                craftedObject.transform.SetParent(playerBrain.Inventory.ObjectHoldAnchor);
                craftedObject.transform.localPosition = Vector3.zero;
            }

            if (onPickSequencer != null)
            {
                StartCoroutine(PlayOnPickSequencer(isNew));
            }
            else
            {
                ShowNewRewardView(isNew);
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator PlayOnPickSequencer(bool isNew)
    {
        yield return StartCoroutine(onPickSequencer.ExecuteSequence());

        ShowNewRewardView(isNew);
        Destroy(gameObject);
    }

    private void ShowNewRewardView(bool isNew)
    {
        if (isNew)
        {
            managerRefs.UIManager.ToggleRewardView(true, this);
        }
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
        OnTargetedEvent?.Invoke(this);
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
        OnUnTargetedEvent?.Invoke(this);
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OnGetReward(ManagerRefs managerRefs, GameObject giver = null)
    {
    }

    public IRewardable.UIDisplayData GetRewardDisplayData()
    {
        return new IRewardable.UIDisplayData
        {
            DisplayAbovePlayer = false,
            DisplayName = craftedObjectRecipe.CraftedName,
            Icon = craftedObjectRecipe.CraftedSprite,
            HighlightColor = craftedObjectRecipe.Rarity.RarityColor
        };
    }
}
