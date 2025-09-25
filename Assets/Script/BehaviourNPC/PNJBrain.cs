using Alchemy.Inspector;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.UI;

public class PNJBrain : MonoBehaviour, IInteractable
{
    [SerializeField] private BehaviorGraphAgent agent;

    [SerializeField] private ManagerRefs managerRefs;
    [SerializeField] private Sprite interactIcon;

    [SerializeField] private SpriteRenderer stateIconDisplay;

    private PNJBehaviour PNJInfos;
    private BlackboardVariable<PnjEvent> pnjBuying;
    private BlackboardVariable<PnjEvent> pnjArriveBuying;
    private BlackboardVariable<PnjEvent> pnjOutside;

    public Sprite InteractIcon => interactIcon;
    public ManagerRefs ManagerRefs => managerRefs;

    public BehaviorGraphAgent Agent => agent;

    public BlackboardVariable<PnjEvent> PNJBuying => pnjBuying;
    public BlackboardVariable<PnjEvent> PNJArriveBuying => pnjArriveBuying;
    public BlackboardVariable<PnjEvent> PNJOutside => pnjOutside;

    [Button]
    public void TriggerNewBuyingPos ()
    {
        //agent.SetVariableValue<Vector3>("BuyingObjectPos", managerRefs.SellManager.GetRandomSellSlot().transform.position);
        //agent.SetVariableValue<State>("ActualState", State.Buying);
    }

    public void ChangeIcon (Sprite icon)
    {
        stateIconDisplay.sprite = icon;
    }

    public void ChangeState (State state)
    {
        agent.SetVariableValue<State>("ActualState", state);
    }

    public SellSlot RandomChooseSellSlot (List<ECraftedType> prefTypes)
    {
        if (managerRefs.SellManager.GetRandomSellSlot(prefTypes, out SellSlot sellSlot))
        {
            agent.SetVariableValue<Vector3>("BuyingObjectPos", sellSlot.transform.position);
        }
        return sellSlot;
    }

    public void SetBuyTime (float buyTime)
    {
        agent.SetVariableValue<float>("WaitBuy", buyTime);
    }

    public void Setup (PNJData datas)
    {
        if (agent.BlackboardReference.GetVariable("PNJBuying", out pnjBuying))
            pnjBuying.Value.Event += OnPNJBuying;

        if (agent.BlackboardReference.GetVariable("PNJOutside", out pnjOutside))
            pnjOutside.Value.Event += OnPNJOutside;

        agent.BlackboardReference.GetVariable("PNJArriveBuying", out pnjArriveBuying);

        PNJInfos = datas.GetStats();
        PNJInfos.OnSpawn(this);
        transform.name = PNJInfos.PNJData.name;
        managerRefs.PNJManager.AddPnj(this);
    }

    private void Update()
    {
        PNJInfos.OnUpdate(this);
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
        if (agent.BlackboardReference.GetVariable<State>("ActualState", out BlackboardVariable<State> state) && state.Value != State.GoOut)
        {
            agent.SetVariableValue<State>("ActualState", State.Stop);
        }
        return true;
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        if (agent.BlackboardReference.GetVariable<State>("ActualState", out BlackboardVariable<State> state) && state.Value != State.GoOut)
        {
            agent.SetVariableValue<State>("ActualState", State.RoamingAround);
        }
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }
}
