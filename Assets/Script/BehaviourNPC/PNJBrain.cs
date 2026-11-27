using Alchemy.Inspector;
using System;
using System.Collections.Generic;
using TMPEffects.TMPEvents;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class PNJBrain : WorldViewInteract<DialogueView>
{
    private const string OutsidePosVariable = "OutsidePos";
    private const string ShopPosVariable = "ShopPos";
    private const string ShopDurationVariable = "ShopDuration";
    private const string BuyingPosition = "BuyingObjectPos";
    private const string PauseForceLeave = "PauseForceLeave";
    private const string PauseTimeLeave = "PauseTimeLeave";
    #region Variables

    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite questIcon;
    [SerializeField] private Collider collider;
    [SerializeField] private WorldSpeech worldSpeech;
    [SerializeField] private PNJInfoData startData;
    [SerializeField] private float interactStopMaxDistance = 0.5f;
    [SerializeField] private Material outlineMat;
    [SerializeField] private Renderer mainRenderer;
    [SerializeField] private bool canGoOut = true;

    private Material baseMat;
    private PNJInfoData PNJBaseData;
    private PNJRuntimeData PNJRuntime;
    private List<QuestInfoSO> givenQuests = new List<QuestInfoSO>();
    private BlackboardVariable<PnjEvent> pnjBuying;
    private BlackboardVariable<PnjEvent> pnjArriveBuying;
    private BlackboardVariable<PnjEvent> pnjOutside;
    private BlackboardVariable<PnjEvent> pnjStartBuying;
    private DialogueData currentMainDialogue;
    private bool requestOut;

    public PNJRuntimeData Data => PNJRuntime;
    public ManagerRefs ManagerRefs => managerRefs;
    public BehaviorGraphAgent Agent => agent;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public BlackboardVariable<PnjEvent> PNJBuying => pnjBuying;
    public BlackboardVariable<PnjEvent> PNJArriveBuying => pnjArriveBuying;
    public BlackboardVariable<PnjEvent> PNJOutside => pnjOutside;
    public BlackboardVariable<PnjEvent> PNJStartBuying => pnjStartBuying;

    public bool HasQuest => givenQuests.Count > 0;
    public bool ShouldReturn => PNJRuntime.IsSpecial || HasQuest;
    public WorldSpeech WorldSpeech => worldSpeech;

    #endregion

    private void Start()
    {
        baseMat = mainRenderer.material;

        if (startData != null)
        {
            Setup(startData);
        }
    }

    private void Update()
    {
        foreach (IPNJTraitRuntime traitRuntime in PNJRuntime.ActiveTraits)
        {
            traitRuntime.OnUpdate(this);
        }

        if (requestOut && agent.BlackboardReference.GetVariable<bool>(PauseForceLeave, out BlackboardVariable<bool> pauseLeaving) && !pauseLeaving)
        {
            requestOut = false;
            ChangeState(State.GoOut);
        }
    }

    protected override void OnDestroy()
    {
        foreach (IPNJTraitRuntime traitRuntime in PNJRuntime.ActiveTraits)
        {
            traitRuntime.OnDespawn(this);
        }

        if (pnjBuying != null)
            pnjBuying.Value.Event -= OnPNJBuying;

        if (pnjOutside != null)
            pnjOutside.Value.Event -= OnPNJOutside;

        managerRefs.GameEventsManager.questEvents.OnFinishQuest -= OnFinishQuest;
        managerRefs.GameEventsManager.dayEvents.OnNearEndDay -= ForceDayEnd;

        base.OnDestroy();
    }

    [Button]
    public void Setup(PNJInfoData datas)
    {
        if (agent.BlackboardReference.GetVariable("PNJBuying", out pnjBuying))
            pnjBuying.Value.Event += OnPNJBuying;

        if (agent.BlackboardReference.GetVariable("PNJOutside", out pnjOutside))
            pnjOutside.Value.Event += OnPNJOutside;

        agent.BlackboardReference.GetVariable("PNJStartBuying", out pnjStartBuying);
        agent.BlackboardReference.GetVariable("PNJArriveBuying", out pnjArriveBuying);

        PNJBaseData = datas;
        PNJRuntime = datas.GetRuntimeData();
        foreach (IPNJTraitRuntime traitRuntime in PNJRuntime.ActiveTraits)
        {
            traitRuntime.OnSpawn(this);
        }

        transform.name = PNJRuntime.Identity.Name;
        currentMainDialogue = PNJRuntime.Identity.Dialogue;
        
        if (PNJRuntime.Identity.Controller != null)
        {
            animator.runtimeAnimatorController = PNJRuntime.Identity.Controller;
        }

        Agent.SetVariableValue<Vector3>(OutsidePosVariable, managerRefs.PNJManager.PnjSpawnOutside);
        Agent.SetVariableValue<Vector3>(ShopPosVariable, managerRefs.PNJManager.PnjShopStop);
        Agent.SetVariableValue<float>(ShopDurationVariable, PNJRuntime.ShopStayDuration);

        managerRefs.GameEventsManager.questEvents.OnFinishQuest += OnFinishQuest;
        managerRefs.GameEventsManager.questEvents.OnQuestStateChange += OnQuestStateChange;
        managerRefs.GameEventsManager.dayEvents.OnNearEndDay += ForceDayEnd;
    }

    private void ForceDayEnd() 
    {
        if (!canGoOut)
            return;

        requestOut = true;
    }

    public void ChangeMainDialogue(DialogueData newDialogue)
    {
        currentMainDialogue = newDialogue;
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
            sellSlot.SetBuyLock(true);
            agent.SetVariableValue<Vector3>(BuyingPosition, sellSlot.BuyPosition.position);
        }
        return sellSlot;
    }

    public void SetBuyTime (float buyTime)
    {
        agent.SetVariableValue<float>("WaitBuy", buyTime);
    }

    [Button]
    public void SetPauseTimeLeave(bool isPaused)
    {
        if (agent == null)
            return;

        agent.SetVariableValue<bool>(PauseTimeLeave, isPaused);
    }

    public void SetPauseForceLeave(bool isPaused)
    {
        if (agent == null)
            return;

        agent.SetVariableValue<bool>(PauseForceLeave, isPaused);
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
            SetPauseTimeLeave(true);
        }
    }

    public bool HasGivenQuest(string questID)
    {
        foreach (QuestInfoSO questInfo in givenQuests)
        {
            if (questInfo.ID == questID)
            {
                return true;
            }
        }
        return false;
    }

    public QuestInfoSO GetGivenQuest(string questID)
    {
        foreach (QuestInfoSO questInfo in givenQuests)
        {
            if (questInfo.ID == questID)
            {
                return questInfo;
            }
        }
        return null;
    }

    private void OnQuestStateChange(Quest quest)
    {
        if (givenQuests.Contains(quest.Info) && quest.State == QuestState.CAN_FINISH)
        {
            worldSpeech.DisplaySpeech("?", true);
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
                    SetPauseTimeLeave(false);
                break;
            }
        }

        if (!TryGetRedeemQuest(out Quest data))
        {
            worldSpeech.StopSpeech();
        }
    }
    private bool TryGetRedeemQuest(out Quest data)
    {
        foreach (QuestInfoSO questInfo in givenQuests)
        {
            Quest quest = managerRefs.QuestManager.GetQuestById(questInfo.ID);
            if (quest.State == QuestState.CAN_FINISH)
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
    }

    #region IInteractable

    public override void StartInteract(PlayerBrain playerBrain)
    {
        foreach (IPNJTraitRuntime trait in PNJRuntime.ActiveTraits)
        {
            trait.OnInteract(this);
        }

        if (TryGetRedeemQuest(out Quest data) && data.Info.FinishedDialogueData != null)
        {
            ManagerRefs.GameEventsManager.questEvents.FinishQuest(data.Info.ID);
            ManagerRefs.DialogueManager.StartDialogue(data.Info.FinishedDialogueData, this);

            if (data.Info.ReplaceDialogueData != null)
            {
                ChangeMainDialogue(data.Info.ReplaceDialogueData);
            }
        }
        else
        {
            ManagerRefs.DialogueManager.StartDialogue(currentMainDialogue, this);
        }

        base.StartInteract(playerBrain);
    }

    public override bool CanInteract(PlayerBrain playerBrain)
    {
        if (agent.BlackboardReference.GetVariable<State>("ActualState", out BlackboardVariable<State> state))
        {
            Vector3 vectorToPlayer = transform.position - playerBrain.transform.position;
            vectorToPlayer.y = 0;

            if (vectorToPlayer.magnitude <= interactStopMaxDistance && state.Value != State.GoOut && state.Value != State.Buying)
            {
                agent.SetVariableValue<State>("ActualState", State.Stop);
                SetPauseTimeLeave(true);
            }

            return state.Value == State.RoamingAround || state.Value == State.Stop;
        }

        return true;
    }

    public override void UnTargeted(PlayerBrain playerBrain)
    {
        mainRenderer.material = baseMat;

        if (agent.BlackboardReference.GetVariable<State>("ActualState", out BlackboardVariable<State> state) && state.Value != State.GoOut)
        {
            agent.SetVariableValue<State>("ActualState", State.RoamingAround);
            if (givenQuests.Count <= 0)
            {
                SetPauseTimeLeave(false);
            }
        }

        base.UnTargeted(playerBrain);
    }

    public override void OnTargeted(PlayerBrain playerBrain)
    {
        base.OnTargeted(playerBrain);
        mainRenderer.material = outlineMat != null ? outlineMat : baseMat;
    }

    #endregion
}
