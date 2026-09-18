using Coffee.UIEffects;
using System;
using UnityEngine;

[Serializable]
public abstract class Objective : ScriptableObject
{
    public int ReputationAmount;
    public string Name;
    public string Description;
    public Sprite Icon;
    public UIEffectPreset BGPreset;
    public ManagerRefs Refs;
    public abstract void Subscribe(ManagerRefs refs);
    public abstract void Unsubscribe(ManagerRefs refs);
}