using System;
using UnityEngine;

public abstract class QuestStepData : ScriptableObject
{
    public abstract QuestStepRuntime GetRuntimeLogic();
    public abstract QuestStepDisplayData QuestStepUIDisplayData { get; }

    [Serializable]
    public class QuestStepDisplayData
    {
        public string Name;
        public string Description;
    }
}
