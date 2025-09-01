using System;
using TMPEffects.TMPEvents;
using UnityEngine;
public class PNJData : ScriptableObject
{
    public string Name;
    public string Description;
    public ManagerRefs ManagerRefs;
    public Sprite Portrait;

    public virtual PNJBehaviour GetStats ()
    {
        PNJBehaviour stats = new PNJBehaviour(this);
        return stats;
    }
}

[Serializable]
public class PNJBehaviour
{
    protected PNJData data;

    public PNJData PNJData => data;

    public PNJBehaviour (PNJData data)
    {
        this.data = data;
    }

    public virtual void OnInteract(PNJBrain pnjBrain) { }

    public virtual void OnSpawn(PNJBrain pnjBrain) { }

    public virtual void OnDespawn (PNJBrain pnjBrain) { }

    public virtual void OnTextEvent(TMPEventArgs args) { }
}
