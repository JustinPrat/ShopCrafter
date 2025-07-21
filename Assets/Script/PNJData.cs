using System;
using UnityEngine;
public class PNJData : ScriptableObject
{
    public string Name;
    public string Description;

    public virtual PNJStats GetStats ()
    {
        PNJStats stats = new PNJStats ();
        stats.Name = Name;
        stats.Description = Description;
        return stats;
    }
}

[Serializable]
public class PNJStats
{
    public string Name;
    public string Description;

    public virtual void OnInteract(PNJBrain pnjBrain) { }

    public virtual void OnSpawn(PNJBrain pnjBrain) { }

    public virtual void OnDespawn (PNJBrain pnjBrain) { }
}
