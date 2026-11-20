using System;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public class Identity
{
    public string Name;
    public string Description;
    public Sprite Portrait;
    public AnimatorController Controller;
    public DialogueData Dialogue;
}

public abstract class IdentityData : ScriptableObject
{
    public abstract Identity GetIdentity();
    public abstract IdentityFactory GetFactory();
}