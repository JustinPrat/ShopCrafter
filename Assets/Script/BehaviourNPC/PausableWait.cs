using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, Unity.Properties.GeneratePropertyBag]
[NodeDescription(name: "Pausable Wait", story: "Wait [duration] seconds (Pause if [isPaused] is true)", category: "Time", id: "custom/pausable_wait")]
public partial class PausableWait : Action
{
    [SerializeReference] public BlackboardVariable<float> Duration;
    [SerializeReference] public BlackboardVariable<bool> IsPaused;

    private float timer;

    protected override Status OnStart()
    {
        timer = 0f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsPaused.Value)
        {
            return Status.Running;
        }

        timer += Time.deltaTime;

        if (timer >= Duration.Value)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}