using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set NavMeshAgent Area Mask", story: "Set [PNJ] AreaMask [Mask] To [Enabled]", category: "Action", id: "eb30da4c78820ea70f93cd155eff4286")]
public partial class SetNavMeshAgentAreaMaskAction : Action
{
    [SerializeReference] public BlackboardVariable<PNJBrain> PNJ;
    [SerializeReference] public BlackboardVariable<string> Mask;
    [SerializeReference] public BlackboardVariable<bool> Enabled;

    protected override Status OnStart()
    {
        if (PNJ.Value == null)
        {
            Debug.LogError("PNJ manquant pour set nav mesh agent area mask");
            return Status.Failure;
        }

        int areaIndex = NavMesh.GetAreaFromName(Mask.Value);

        if (areaIndex == -1)
        {
            Debug.LogError($"L'Area '{Mask.Value}' n'existe pas dans le PNJ behavior");
            return Status.Failure;
        }

        if (Enabled.Value)
        {
            PNJ.Value.NavMeshAgent.areaMask |= (1 << areaIndex);
        }
        else
        {
            PNJ.Value.NavMeshAgent.areaMask &= ~(1 << areaIndex);
        }

        return Status.Success;
    }
}

