using UnityEngine;

public class PlayerManager : MonoBehaviour, IInitializableManager
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private PlayerBrain playerBrain;

    public PlayerBrain PlayerBrain => playerBrain;

    public void Register(ManagerRefs refs)
    {
        managerRefs.PlayerManager = this;
    }

    public void Init(ManagerRefs refs)
    {
    }

    public bool HasCraftedItem(CraftedObjectRecipe recipe)
    {
        return playerBrain.Inventory.HasItemInSelectedSlot(recipe);
    }

    public void ConsumeCraftedItem()
    {
        playerBrain.Inventory.ConsumeHeldItem();
    }

    public void SetPlayer(PlayerBrain playerBrain)
    {
        this.playerBrain = playerBrain;
    }
}
