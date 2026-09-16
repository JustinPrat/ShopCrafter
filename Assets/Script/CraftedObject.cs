using System;
using TriInspector;
using UnityEngine;

public class CraftedObject : MonoBehaviour, IInteractable, IRewardable
{
    [SerializeField]
    private string interactText;

    [ReadOnly, SerializeField]
    private CraftedObjectData craftedObjectData;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public bool IsLocked { get; set; }
    public int Price => craftedObjectData.GetPrice();
    public CraftedObjectData CraftedData => craftedObjectData;
    public Collider PhysicCollider => collider;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;

    public SpawnedReward RewardPrefab { get; set; }
    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public void Init (CraftedObjectData data)
    {
        craftedObjectData = data;
        spriteRenderer.sprite = data.CraftedObjectRecipe.CraftedSprite;
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return false;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        //transform.SetParent(playerBrain.ObjectHoldAnchor);
        //transform.localPosition = Vector3.zero;
        //playerBrain.TryHoldItem(this);
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
        OnUnTargetedEvent?.Invoke(this);
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
        OnTargetedEvent?.Invoke(this);
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
        return new IRewardable.UIDisplayData()
        {
            DisplayAbovePlayer = false,
            DisplayName = craftedObjectData.CraftedObjectRecipe.CraftedName,
            Icon = craftedObjectData.CraftedObjectRecipe.CraftedSprite,
            HighlightColor = craftedObjectData.CraftedObjectRecipe.Rarity.RarityColor
        };
    }
}
