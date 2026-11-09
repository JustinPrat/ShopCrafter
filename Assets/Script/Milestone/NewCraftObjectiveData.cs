using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewCraftObjective", menuName = "ShopCrafter/Milestone/NewCraftObjective")]
public class NewCraftObjectiveData : ObjectiveData
{
    public bool RequireNew = true;

    public override ObjectiveBehavior GetObjectiveBehavior()
    {
        NewCraftObjectiveBehavior objectiveBehavior = new NewCraftObjectiveBehavior();
        objectiveBehavior.Setup(this);
        return objectiveBehavior;
    }

    public class NewCraftObjectiveBehavior : ObjectiveBehavior
    {
        private NewCraftObjectiveData data;
        public override ObjectiveData Data => data;

        public void Setup(NewCraftObjectiveData data)
        {
            this.data = data;
        }

        public override void OnStartObjective()
        {
            data.Refs.GameEventsManager.craftEvents.OnCraftedItem += OnCraftedItem;
        }

        private void OnCraftedItem(CraftedObjectData craftedObjectData)
        {
            if (!data.RequireNew || (data.RequireNew && craftedObjectData.IsNew))
            {
                GainReputation(data.ReputationAmount);
            }
        }

        public override void OnStopObjective()
        {
            data.Refs.GameEventsManager.craftEvents.OnCraftedItem -= OnCraftedItem;
        }
    }
}
