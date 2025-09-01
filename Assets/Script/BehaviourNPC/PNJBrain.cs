using Alchemy.Inspector;
using Unity.Behavior;
using UnityEngine;

public class PNJBrain : MonoBehaviour, IInteractable
{
    [SerializeField] private BehaviorGraphAgent agent;

    [SerializeField] private ManagerRefs managerRefs;
    [SerializeField] private Sprite interactIcon;

    private PNJBehaviour PNJInfos;
    private BlackboardVariable<PnjEvent> pnjBuying;
    private BlackboardVariable<PnjEvent> pnjOutside;

    public Sprite InteractIcon => interactIcon;
    public ManagerRefs ManagerRefs => managerRefs;

    public BlackboardVariable<PnjEvent> PNJBuying => pnjBuying;
    public BlackboardVariable<PnjEvent> PNJOutside => pnjOutside;

    [Button]
    public void TriggerNewBuyingPos ()
    {
        agent.SetVariableValue<Vector3>("BuyingObjectPos", managerRefs.SellManager.GetRandomSellSlot().transform.position);
        agent.SetVariableValue<State>("ActualState", State.Buying);
    }

    public void ChangeState (State state)
    {
        agent.SetVariableValue<State>("ActualState", state);
    }

    public SellSlot RandomChooseSellSlot ()
    {
        SellSlot sellSlot = managerRefs.SellManager.GetRandomSellSlot();
        agent.SetVariableValue<Vector3>("BuyingObjectPos", sellSlot.transform.position);
        return sellSlot;
    }

    public void Setup (PNJData datas)
    {
        if (agent.BlackboardReference.GetVariable("PNJBuying", out pnjBuying))
            pnjBuying.Value.Event += OnPNJBuying;

        if (agent.BlackboardReference.GetVariable("PNJOutside", out pnjOutside))
            pnjOutside.Value.Event += OnPNJOutside;

        PNJInfos = datas.GetStats();
        PNJInfos.OnSpawn(this);
        transform.name = PNJInfos.PNJData.name;
        managerRefs.PNJManager.AddPnj(this);
    }

    private void OnDestroy()
    {
        pnjBuying.Value.Event -= OnPNJBuying;
        pnjOutside.Value.Event -= OnPNJOutside;
    }

    private void OnPNJBuying(GameObject caller)
    {
        Debug.Log("On PNJ end pos buying Prouting : " + caller.GetComponent<PNJBrain>().PNJInfos.PNJData.Name + " / receiver : " + PNJInfos.PNJData.Name);
    }

    private void OnPNJOutside(GameObject caller)
    {
        Debug.Log("On PNJ outside");
        managerRefs.PNJManager.RemovePnj(this);
        Destroy(gameObject);
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        Debug.Log("Interact with PNJ");
        PNJInfos.OnInteract(this);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        Debug.Log("Can interact asking");
        agent.SetVariableValue<State>("ActualState", State.Stop);
        return true;
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        agent.SetVariableValue<State>("ActualState", State.RoamingAround);
    }
}
