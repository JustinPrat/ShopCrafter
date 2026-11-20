using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/Quests/CraftItemQuestStepData")]
public class CraftItemQuestStepData : QuestStepData
{
    public CraftedObjectRecipe TargetRecipe;

    [SerializeField]
    private QuestStepDisplayData stepDisplayData;
    public override QuestStepDisplayData QuestStepUIDisplayData => stepDisplayData;

    public override QuestStepRuntime GetRuntimeLogic()
    {
        return new CraftItemQuestStepRuntime(this);
    }
}

public class CraftItemQuestStepRuntime : QuestStepRuntime
{    private CraftItemQuestStepData data;

    public CraftItemQuestStepRuntime(CraftItemQuestStepData data)
    {
        this.data = data;
    }

    public override void InitializeQuestStep(string questId, int stepIndex, string questStepState, ManagerRefs managerRefs, PNJBrain owner)
    {
        base.InitializeQuestStep(questId, stepIndex, questStepState, managerRefs, owner);
        managerRefs.GameEventsManager.craftEvents.OnCraftedItem += OnCraftedItem;
    }

    private void OnCraftedItem(CraftedObjectData itemCrafted)
    {
        if (itemCrafted.CraftedObjectRecipe == data.TargetRecipe)
        {
            FinishQuestStep();
        }
    }

    protected override void FinishQuestStep()
    {
        managerRefs.GameEventsManager.craftEvents.OnCraftedItem -= OnCraftedItem;
        base.FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
    }
}