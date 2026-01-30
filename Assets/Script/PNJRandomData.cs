using TMPEffects.TMPEvents;
using UnityEngine;

public class PNJRandomData : PNJData
{
    public float ShopStayDuration;
    public DialogueData PNJDialogueData;

    public override PNJBehaviour GetStats()
    {
        PNJRandomBehaviour stats = new PNJRandomBehaviour(this);
        return stats;
    }
}

public class PNJRandomBehaviour : PNJBehaviour
{
    private PNJRandomData currentData => (PNJRandomData)data;

    public PNJRandomBehaviour(PNJRandomData data) : base(data)
    {
        this.data = data;
    }

    public override void OnTextEvent(TMPEventArgs args)
    {
        base.OnTextEvent(args);
    }

    public override void OnSpawn(PNJBrain pnjBrain)
    {
        base.OnSpawn(pnjBrain);
        pnjBrain.Agent.SetVariableValue<float>("ShopDuration", currentData.ShopStayDuration);
        pnjBrain.Agent.SetVariableValue<Vector3>("OutsidePos", managerRefs.PNJManager.PnjSpawnOutside);
    }

    public override void OnInteract(PNJBrain pnjBrain)
    {
        base.OnInteract(pnjBrain);
        //data.ManagerRefs.DialogueManager.StartDialogue(currentData.PNJDialogueData, this);
    }
}
