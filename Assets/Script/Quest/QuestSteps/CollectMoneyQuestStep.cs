using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/Quests/CollectMoney")]
public class CollectMoneyQuestStep : QuestStepData
{
    [SerializeField]
    private int moneyAmount;

    public override QuestStepRuntime GetRuntimeLogic()
    {
        return new CollectMoneyQuestStepRuntime(new MoneyCollectData { MoneyAmount = moneyAmount });
    }
}

public struct MoneyCollectData
{
    public int MoneyAmount;
}

public class CollectMoneyQuestStepRuntime : QuestStepRuntime
{
    private MoneyCollectData data;
    private int collectedMoney;

    public CollectMoneyQuestStepRuntime(MoneyCollectData data)
    {
        this.data = data;
    }

    public override void InitializeQuestStep(string questId, int stepIndex, string questStepState, ManagerRefs managerRefs)
    {
        base.InitializeQuestStep(questId, stepIndex, questStepState, managerRefs);
        managerRefs.GameEventsManager.OnMoneyGained += OnMoneyGained;
    }

    private void OnMoneyGained (int amount)
    {
        managerRefs.GameEventsManager.OnMoneyGained -= OnMoneyGained;
        collectedMoney += amount;

        if (collectedMoney > data.MoneyAmount)
        {
            FinishQuestStep();
        }
    }

    protected override void SetQuestStepState(string state)
    {
    }
}


