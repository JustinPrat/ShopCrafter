using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerUIButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI textUI;

    private Answer currentAnswer;

    public Action<Answer> OnAnswerClicked;

    public void Setup (Answer answer)
    {
        textUI.text = answer.Line;
        currentAnswer = answer;
        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    private void OnClick ()
    {
        OnAnswerClicked?.Invoke(currentAnswer);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
