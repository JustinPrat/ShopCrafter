using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerUIButton : MonoBehaviour
{
    [SerializeField] private AdvancedButton button;
    [SerializeField] private TextMeshProUGUI textUI;

    private Answer currentAnswer;

    public Action<Answer> OnAnswerClicked;

    public void Setup (Answer answer)
    {
        textUI.text = answer.Line;
        currentAnswer = answer;
        button.OnLeftClick.RemoveListener(OnClick);
        button.OnLeftClick.AddListener(OnClick);
    }

    private void OnClick (AdvancedButton button)
    {
        OnAnswerClicked?.Invoke(currentAnswer);
    }

    private void OnDestroy()
    {
        button.OnLeftClick.RemoveAllListeners();
    }
}
