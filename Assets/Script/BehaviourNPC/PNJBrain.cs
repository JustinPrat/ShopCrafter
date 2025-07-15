using Alchemy.Inspector;
using Unity.Behavior;
using UnityEngine;

public class PNJBrain : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private PnjBuying PNJBuyingEvent;

    [Button] 
    public void TriggerNewBuyingPos ()
    {
        agent.SetVariableValue<Vector3>("BuyingObjectPos", Vector3.zero);
        agent.SetVariableValue<State>("CurrentState", State.Buying);
    }

    private void Start()
    {
        PNJBuyingEvent.Event += PNJBuyingEvent_Event;
    }

    private void PNJBuyingEvent_Event()
    {
        Debug.Log("On PNJ end pos buying");
    }
}
