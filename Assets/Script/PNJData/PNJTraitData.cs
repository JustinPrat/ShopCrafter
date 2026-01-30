using UnityEngine;

public abstract class PNJTraitData : ScriptableObject
{
    public abstract IPNJTraitRuntime GetRuntimeLogic();
}