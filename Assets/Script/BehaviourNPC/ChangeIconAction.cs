using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeIcon", story: "Change [icon] of [PNJ]", category: "Action", id: "6c4d63222aa23fc5761e789899c40dae")]
public partial class ChangeIconAction : Action
{
    [SerializeReference] public BlackboardVariable<Sprite> Icon;
    [SerializeReference] public BlackboardVariable<PNJBrain> PNJ;
    protected override Status OnStart()
    {
        //PNJ.Value.ChangeIcon(Icon.Value);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

