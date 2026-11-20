using System;
using System.Collections;
using UnityEngine;

public class PickCraftedObject : BaseWorldInteract, IRewardable
{
    [SerializeField]
    private CraftedObjectRecipe craftedObjectRecipe;

    [SerializeField]
    private Sequencer.Sequencer onPickSequencer;

    [SerializeField]
    private SpawnedReward rewardPrefab;
    public SpawnedReward RewardPrefab { get => rewardPrefab; set => rewardPrefab = value; }

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        if (playerBrain.Inventory.HasEmptySpace())
            return true;

        return false;
    }

    public override void StartInteract(PlayerBrain playerBrain)
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

            OnStartInteractEvent?.Invoke(this);
        }
    }

    private IEnumerator PlayOnPickSequencer(bool isNew)
    {
        isInteracting = true;
        yield return StartCoroutine(onPickSequencer.ExecuteSequence());
        isInteracting = false;
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
