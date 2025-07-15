using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RoamingAround", story: "[Agent] roaming around from [raduis]", category: "Action", id: "651693f6e3842268d94330b5c57d281f")]
public partial class RoamingAroundAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Raduis;

    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {
        navMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(RandomNavSphere(Agent.Value.transform.position, Raduis.Value, -1));
        Debug.Log("Start current random destination : " + navMeshAgent.destination);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (navMeshAgent.remainingDistance <= 0.1f)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        Debug.Log("Arrived at random position : " + navMeshAgent.transform.position);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}

