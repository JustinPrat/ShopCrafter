using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private PlayerBrain playerBrain;

    private void Awake()
    {
        managerRefs.PlayerManager = this;
    }

    public bool HasCraftedItem(CraftedObjectRecipe recipe)
    {
        return playerBrain.Inventory.HasItemInSelectedSlot(recipe);
    }

    public void ConsumeCraftedItem()
    {
        playerBrain.Inventory.ConsumeHeldItem();
    }
}
