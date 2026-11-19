using TriInspector;
using UnityEngine;

public class CraftedObject : BaseWorldInteract, IRewardable
{
    [ReadOnly, SerializeField]
    private CraftedObjectData craftedObjectData;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public int Price => craftedObjectData.GetPrice();
    public CraftedObjectData CraftedData => craftedObjectData;
    public SpawnedReward RewardPrefab { get; set; }

    public void Init (CraftedObjectData data)
    {
        craftedObjectData = data;
        spriteRenderer.sprite = data.CraftedObjectRecipe.CraftedSprite;
    }

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        return false;
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
