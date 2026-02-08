using UnityEngine;

public abstract class QuestStepData : ScriptableObject
{
    public abstract QuestStepRuntime GetRuntimeLogic();
}
