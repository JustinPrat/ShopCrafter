using Coffee.UIEffects;
using System;
using UnityEngine;

[Serializable]
public abstract class ObjectiveData : ScriptableObject
{
    public int ReputationAmount;
    public int ReputationMax;
    public string Name;
    public string Description;
    public Sprite Icon;
    public UIEffectPreset BGPreset;
    public ManagerRefs Refs;
    public abstract ObjectiveBehavior GetObjectiveBehavior();
}

[Serializable]
public abstract class ObjectiveBehavior
{
    public int ReputationGained;
    public abstract ObjectiveData Data { get; }
    public virtual void OnStartObjective() { }
    public virtual void OnStopObjective() { }
    protected void GainReputation(int amount)
    {
        if (ReputationGained + amount > Data.ReputationMax)
        {
            ReputationGained = Data.ReputationMax;
            return;
        }

        ReputationGained += amount;
        Data.Refs.GameEventsManager.milestoneEvents.GainReputation(amount);
    }
}