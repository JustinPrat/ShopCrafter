using Alchemy.Inspector;
using TMPro;
using UnityEngine;

public class WorldSpeech : MonoBehaviour
{
    private const string SpeechBool = "Speech";

    [SerializeField] private TextMeshProUGUI speechText;
    [SerializeField] private float speechDuration;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject activatedSpeechObject;

    private bool isSpeechDisplayed;
    private float speechTimerEnd;
    private bool speechAlwaysDisplay;

#if UNITY_EDITOR
    [Button]
    private void DisplaySpeechTest(string speech)
    {
        DisplaySpeech(speech);
    }
#endif

    private void Update()
    {
        if (isSpeechDisplayed && speechTimerEnd <= Time.time && !speechAlwaysDisplay)
        {
            StopSpeech();
        }
    }

    public void DisplaySpeech(string text, bool alwaysDisplay = false)
    {
        activatedSpeechObject.SetActive(true);
        animator.SetBool(SpeechBool, true);
        speechText.text = text;
        isSpeechDisplayed = true;
        speechTimerEnd = Time.time + speechDuration;
        speechAlwaysDisplay = alwaysDisplay;
    }


    public void DisplaySpeech(string text, float duration)
    {
        activatedSpeechObject.SetActive(true);
        animator.SetBool(SpeechBool, true);
        speechText.text = text;
        isSpeechDisplayed = true;
        speechTimerEnd = Time.time + duration;
        speechAlwaysDisplay = false;
    }

    public void StopSpeech()
    {
        activatedSpeechObject.SetActive(false);
        animator.SetBool(SpeechBool, false);
        speechText.text = "";
        isSpeechDisplayed = false;
    }
}
