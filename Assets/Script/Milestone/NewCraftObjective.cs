using UnityEngine;

[CreateAssetMenu(fileName = "NewCraftObjective", menuName = "ShopCrafter/Milestone/NewCraftObjective")]
public class NewCraftObjective : Objective
{
    public override void Subscribe(ManagerRefs refs)
    {
        refs.GameEventsManager.craftEvents.OnCraftedItem += OnCraftedItem;
    }

    private void OnCraftedItem(CraftedObjectData craftedObjectData)
    {
        if (craftedObjectData.IsNew)
        {
            Refs.GameEventsManager.milestoneEvents.GainReputation(ReputationAmount);
        }
    }

    public override void Unsubscribe(ManagerRefs refs)
    {
        refs.GameEventsManager.craftEvents.OnCraftedItem -= OnCraftedItem;
    }
}
