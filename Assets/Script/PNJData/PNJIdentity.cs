using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Identity
{
    public string Name;
    public string Description;
    public Sprite Portrait;
    public DialogueData Dialogue;
}

public abstract class IdentityData : ScriptableObject
{
    public abstract Identity GetIdentity();
}