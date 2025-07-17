using Alchemy.Inspector;
using Unity.Behavior;
using UnityEngine;

public class PNJBrain : MonoBehaviour, IInteractable
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private PNJEvents PNJBuyingEvent;
    [SerializeField] private PNJEvents PNJOutsideEvent;

    [SerializeField] private ManagerRefs managerRefs;
    [SerializeField] private Sprite interactIcon;

    public Sprite InteractIcon => interactIcon;

    [Button]
    public void TriggerNewBuyingPos ()
    {
        agent.SetVariableValue<Vector3>("BuyingObjectPos", Vector3.zero);
        agent.SetVariableValue<State>("CurrentState", State.Buying);
    }

    private void Start()
    {
        PNJBuyingEvent.Event += OnPNJBuying;
        PNJOutsideEvent.Event += OnPNJOutside;

        managerRefs.PNJManager.AddPnj(this);
    }

    private void OnDestroy()
    {
        PNJBuyingEvent.Event -= OnPNJBuying;
        PNJOutsideEvent.Event -= OnPNJOutside;
    }

    private void OnPNJBuying()
    {
        Debug.Log("On PNJ end pos buying");
    }

    private void OnPNJOutside()
    {
        Debug.Log("On PNJ outside");
        managerRefs.PNJManager.RemovePnj(this);
        Destroy(gameObject);
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        Debug.Log("Interact with PNJ");
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        Debug.Log("Can interact asking");
        agent.SetVariableValue<State>("CurrentState", State.Stop);
        return true;
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        agent.SetVariableValue<State>("CurrentState", State.RoamingAround);
    }
}
