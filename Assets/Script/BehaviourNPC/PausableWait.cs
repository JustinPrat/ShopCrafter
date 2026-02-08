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

    private float _timer;

    protected override Status OnStart()
    {
        _timer = 0f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsPaused.Value)
        {
            return Status.Running;
        }

        _timer += Time.deltaTime;

        if (_timer >= Duration.Value)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}