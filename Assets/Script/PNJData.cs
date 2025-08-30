using System;
using UnityEngine;
public class PNJData : ScriptableObject
{
    public string Name;
    public string Description;
    public ManagerRefs ManagerRefs;

    public virtual PNJStats GetStats ()
    {
        PNJStats stats = new PNJStats(this);
        return stats;
    }
}

[Serializable]
public class PNJStats
{
    private PNJData data;

    public PNJData PNJData => data;

    public PNJStats (PNJData data)
    {
        this.data = data;
    }

    public virtual void OnInteract(PNJBrain pnjBrain) { }

    public virtual void OnSpawn(PNJBrain pnjBrain) { }

    public virtual void OnDespawn (PNJBrain pnjBrain) { }
}
