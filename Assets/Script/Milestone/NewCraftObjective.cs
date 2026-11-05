using UnityEngine;

[CreateAssetMenu(fileName = "NewCraftObjective", menuName = "ShopCrafter/Milestone/NewCraftObjective")]
public class NewCraftObjective : Objective
{
    public bool RequireNew = true;

    public override void Subscribe(ManagerRefs refs)
    {
        refs.GameEventsManager.craftEvents.OnCraftedItem += OnCraftedItem;
    }

    private void OnCraftedItem(CraftedObjectData craftedObjectData)
    {
        if (!RequireNew || (RequireNew && craftedObjectData.IsNew))
        {
            Refs.GameEventsManager.milestoneEvents.GainReputation(ReputationAmount);
        }
    }

    public override void Unsubscribe(ManagerRefs refs)
    {
        refs.GameEventsManager.craftEvents.OnCraftedItem -= OnCraftedItem;
    }
}
