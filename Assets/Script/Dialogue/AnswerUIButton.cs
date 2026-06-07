using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerUIButton : MonoBehaviour
{
    [SerializeField] private AdvancedButton button;
    [SerializeField] private TextMeshProUGUI textUI;

    [SerializeField] private GameObject costUI;
    [SerializeField] private TextMeshProUGUI costAmountText;
    [SerializeField] private Image costSpriteImage;

    [SerializeField] private ManagerRefs managerRefs;

    private Answer currentAnswer;
    private bool clickable;
    private bool hasCost;

    public Action<Answer> OnAnswerClicked;
    public Selectable Selectable => button;

    private void Awake()
    {
        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.OnMoneyUpdated += OnMoneyUpdated;
        }
    }

    public void Setup (Answer answer)
    {
        textUI.text = answer.Line;
        currentAnswer = answer;
        clickable = true;

        button.OnLeftClick.RemoveListener(OnClick);
        button.OnLeftClick.AddListener(OnClick);

        UpdateAnswerState();

        costUI.SetActive(hasCost);
        if (hasCost)
        {
            ICost.UIDisplayData data = answer.Cost.Value.GetCostDisplayData();
            costAmountText.text = data.Amount.ToString();
            costSpriteImage.sprite = data.Icon;
        }
    }

    private void OnMoneyUpdated(int newAmount)
    {
        UpdateAnswerState();
    }

    private void UpdateAnswerState()
    {
        if (currentAnswer.Cost != null && currentAnswer.Cost.Value != null)
        {
            hasCost = true;

            if (currentAnswer.Cost.Value.CanPay(managerRefs))
            {
                clickable = true;
                button.interactable = true;
            }
            else
            {
                clickable = false;
                button.interactable = false;
            }
        }
        else
        {
            hasCost = false;
            clickable = true;
            button.interactable = true;
        }
    }

    private void OnClick (AdvancedButton button)
    {
        if (clickable)
        {
            if (hasCost)
            {
                currentAnswer.Cost.Value.ResolveCost(managerRefs);
            }

            OnAnswerClicked?.Invoke(currentAnswer);
        }
    }

    private void OnDestroy()
    {
        button.OnLeftClick.RemoveAllListeners();
        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.OnMoneyUpdated -= OnMoneyUpdated;
        }
    }
}
