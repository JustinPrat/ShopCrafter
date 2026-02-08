using Alchemy.Inspector;
using System.Collections.Generic;
using TMPEffects.TMPEvents;
using Unity.Behavior;
using UnityEngine;

public class PNJBrain : MonoBehaviour, IInteractable
{
    #region Variables

    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private ManagerRefs managerRefs;
    [SerializeField] private Sprite interactIcon;
    [SerializeField] private SpriteRenderer stateIconDisplay;
    [SerializeField] private Sprite questIcon;

    private PNJInfoData PNJBaseData;
    private PNJRuntimeData PNJRuntime;
    private List<QuestInfoSO> givenQuests = new List<QuestInfoSO>();
    private BlackboardVariable<PnjEvent> pnjBuying;
    private BlackboardVariable<PnjEvent> pnjArriveBuying;
    private BlackboardVariable<PnjEvent> pnjOutside;

    public PNJRuntimeData Data => PNJRuntime;
    public Sprite InteractIcon => interactIcon;
    public ManagerRefs ManagerRefs => managerRefs;
    public BehaviorGraphAgent Agent => agent;
    public BlackboardVariable<PnjEvent> PNJBuying => pnjBuying;
    public BlackboardVariable<PnjEvent> PNJArriveBuying => pnjArriveBuying;
    public BlackboardVariable<PnjEvent> PNJOutside => pnjOutside;

    #endregion

    private void Update()
    {
        foreach (IPNJTraitRuntime traitRuntime in PNJRuntime.ActiveTraits)
        {
            traitRuntime.OnUpdate(this);
        }
    }

    private void OnDestroy()
    {
        foreach (IPNJTraitRuntime traitRuntime in PNJRuntime.ActiveTraits)
        {
            traitRuntime.OnDespawn(this);
        }

        pnjBuying.Value.Event -= OnPNJBuying;
        pnjOutside.Value.Event -= OnPNJOutside;
        managerRefs.GameEventsManager.questEvents.onFinishQuest -= OnFinishQuest;
    }

    public void Setup(PNJInfoData datas)
    {
        if (agent.BlackboardReference.GetVariable("PNJBuying", out pnjBuying))
            pnjBuying.Value.Event += OnPNJBuying;

        if (agent.BlackboardReference.GetVariable("PNJOutside", out pnjOutside))
            pnjOutside.Value.Event += OnPNJOutside;

        agent.BlackboardReference.GetVariable("PNJArriveBuying", out pnjArriveBuying);

        PNJBaseData = datas;
        PNJRuntime = datas.GetRuntimeData();
        foreach (IPNJTraitRuntime traitRuntime in PNJRuntime.ActiveTraits)
        {
            traitRuntime.OnSpawn(this);
        }

        transform.name = PNJRuntime.Identity.Name;
        managerRefs.PNJManager.AddPnj(this);

        Agent.SetVariableValue<float>("ShopDuration", PNJRuntime.ShopStayDuration);
        Agent.SetVariableValue<Vector3>("OutsidePos", managerRefs.PNJManager.PnjSpawnOutside);

        managerRefs.GameEventsManager.questEvents.onFinishQuest += OnFinishQuest;
        managerRefs.GameEventsManager.questEvents.onQuestStateChange += OnQuestStateChange;
    }

    public void ChangeIcon (Sprite icon)
    {
        stateIconDisplay.sprite = icon;
    }

    #region Agent

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

    [Button]
    public void SetPauseShopLeaving(bool isPaused)
    {
        if (agent == null)
            return;

        agent.SetVariableValue<bool>("PauseLeaving", isPaused);
    }

    #endregion

    #region Quests
    public void GiveQuest (QuestInfoSO questInfo)
    {
        if (!givenQuests.Contains(questInfo))
        {
            givenQuests.Add(questInfo);
        }

        if (givenQuests.Count > 0)
        {
            SetPauseShopLeaving(true);
        }
    }
    private void OnQuestStateChange(Quest quest)
    {
        if (quest.state == QuestState.CAN_FINISH)
        {
            ChangeIcon(questIcon);
        }
    }

    private void OnFinishQuest(string questID)
    {
        foreach (QuestInfoSO questInfo in givenQuests)
        {
            if (questInfo.ID == questID)
            {
                givenQuests.Remove(questInfo);
                if (givenQuests.Count <= 0)
                    SetPauseShopLeaving(false);
                break;
            }
        }

        if (!TryGetRedeemQuest(out Quest data))
        {
            ChangeIcon(null);
        }
    }
    private bool TryGetRedeemQuest(out Quest data)
    {
        foreach (QuestInfoSO questInfo in givenQuests)
        {
            Quest quest = managerRefs.QuestManager.GetQuestById(questInfo.ID);
            if (quest.state == QuestState.CAN_FINISH)
            {
                data = quest;
                return true;
            }
        }

        data = null;
        return false;
    }

    #endregion

    public virtual void OnTextEvent(TMPEventArgs args)
    {
        foreach (IPNJTraitRuntime trait in PNJRuntime.ActiveTraits)
        {
            trait.OnTextEvent(args);
        }
    }

    private void OnPNJBuying(GameObject caller)
    {
    }

    private void OnPNJOutside(GameObject caller)
    {
        managerRefs.PNJManager.RemovePnj(this);
        Destroy(gameObject);
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        foreach (IPNJTraitRuntime trait in PNJRuntime.ActiveTraits)
        {
            trait.OnInteract(this);
        }

        if (TryGetRedeemQuest(out Quest data) && data.info.FinishedDialogueData != null)
        {
            ManagerRefs.GameEventsManager.questEvents.FinishQuest(data.info.ID);
            ManagerRefs.DialogueManager.StartDialogue(data.info.FinishedDialogueData, this);
        }
        else
        {
            ManagerRefs.DialogueManager.StartDialogue(PNJRuntime.Identity.Dialogue, this);
        }
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        if (agent.BlackboardReference.GetVariable<State>("ActualState", out BlackboardVariable<State> state) && state.Value != State.GoOut)
        {
            agent.SetVariableValue<State>("ActualState", State.Stop);
            SetPauseShopLeaving(true);
        }
        return true;
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        if (agent.BlackboardReference.GetVariable<State>("ActualState", out BlackboardVariable<State> state) && state.Value != State.GoOut)
        {
            agent.SetVariableValue<State>("ActualState", State.RoamingAround);
            if (givenQuests.Count <= 0)
            {
                SetPauseShopLeaving(false);
            }
        }
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }
}
