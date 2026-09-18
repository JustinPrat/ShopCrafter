using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReputationBoardView : UIView
{
    [SerializeField]
    private TextMeshProUGUI currentReputationAmount;

    [SerializeField]
    private TextMeshProUGUI targetReputationAmount;

    [SerializeField]
    private TextMeshProUGUI currentReputationLevel;

    [SerializeField]
    private Slider currentReputationSlider;

    [SerializeField]
    private Transform layoutObjectives;

    [SerializeField]
    private ObjectiveUI objectiveUIPrefab;

    [SerializeField]
    private GameObject gaugeObject;

    [SerializeField]
    private GameObject specialObject;

    [SerializeField]
    private List<Transform> clientReputationTicketPoses;

    private List<Transform> availableClientReputationTicketPoses;
    private List<BaseTicketUI> baseTicketUIs = new List<BaseTicketUI>();

    private bool updateTicket = true;

    private void Start()
    {
        managerRefs.GameEventsManager.milestoneEvents.OnTicketReroll += OnTicketReroll;
    }

    private void OnDestroy()
    {
        managerRefs.GameEventsManager.milestoneEvents.OnTicketReroll -= OnTicketReroll;
    }

    private void OnTicketReroll()
    {
        updateTicket = true;
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            managerRefs.GameEventsManager.milestoneEvents.ValidateMilestone();

            foreach (Objective objective in managerRefs.MilestoneManager.CurrentMilestone.Objectives)
            {
                ObjectiveUI objectiveUI = Instantiate(objectiveUIPrefab, layoutObjectives);
                objectiveUI.Setup(objective);
            }

            currentReputationAmount.text = managerRefs.MilestoneManager.CurrentMilestoneReputation.ToString();
            targetReputationAmount.text = managerRefs.MilestoneManager.CurrentMilestone.ReputationRequired.ToString();
            currentReputationLevel.text = "Level " + (managerRefs.MilestoneManager.CurrentMilestoneIndex + 1).ToString();
            currentReputationSlider.value = (float)managerRefs.MilestoneManager.CurrentMilestoneReputation / managerRefs.MilestoneManager.CurrentMilestone.ReputationRequired;

            SetTypeActivation(managerRefs.MilestoneManager.CurrentMilestone.IsSpecialCharacter);

            managerRefs.InputManager.Actions.UI.Cancel.started += CancelPressed;

            if (updateTicket)
            {
                UpdateTicket();
            }
        }
        else
        {
            foreach (Transform child in layoutObjectives)
            {
                Destroy(child.gameObject);
            }

            managerRefs.InputManager.Actions.UI.Cancel.started -= CancelPressed;
        }
    }

    private void UpdateTicket()
    {
        updateTicket = false;

        for (int i = baseTicketUIs.Count - 1; i >= 0; i--)
        {
            BaseTicketUI ticketUI = baseTicketUIs[i];
            Destroy(ticketUI.gameObject);
        }

        baseTicketUIs.Clear();
        availableClientReputationTicketPoses = new List<Transform>(clientReputationTicketPoses);

        foreach (DailyTicketData.DailyTicketBehavior ticketBehavior in managerRefs.MilestoneManager.MilestoneDailyTickets)
        {
            Transform pos = availableClientReputationTicketPoses.GetAndRemoveRandomElement();
            BaseTicketUI ticketUI = Instantiate(ticketBehavior.Data.UIPrefab.gameObject, pos).GetComponent<BaseTicketUI>();
            ticketUI.Setup(ticketBehavior);
            baseTicketUIs.Add(ticketUI);
        }
    }

    private void SetTypeActivation(bool isSpecial)
    {
        gaugeObject.SetActive(!isSpecial);
        specialObject.SetActive(isSpecial);
    }

    private void CancelPressed(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
    {
        managerRefs.UIManager.ToggleReputationBoardView(false);
    }
}
