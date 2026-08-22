using System;
using System.Collections;
using UnityEngine;

public class PickCraftedObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private CraftedObjectRecipe craftedObjectRecipe;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private Sequencer.Sequencer onPickSequencer;

    public bool IsLocked { get; set; }

    public string InteractText => "Take";

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }

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
            managerRefs.UIManager.ToggleRewardView(true, craftedObjectRecipe);
        }
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }
}
