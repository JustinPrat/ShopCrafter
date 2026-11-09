using Alchemy.Inspector;
using TMPEffects.Components;
using TMPro;
using UnityEngine;

public class WorldSpeech : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speechText;
    [SerializeField] private TMPWriter speechWriter;
    [SerializeField] private float speechDuration;
    [SerializeField] private float speechSpeed = 1f;
    [SerializeField] private Sequencer.Sequencer appearSequence;
    [SerializeField] private Sequencer.Sequencer hideSequence;
    [SerializeField] private GameObject activatedSpeechObject;

    private bool isSpeechDisplayed;
    private float speechTimerEnd;
    private bool speechAlwaysDisplay;
    public bool IsSpeechDisplayed => isSpeechDisplayed;

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

    public void UpdateSpeech(string speech, bool needReWrite = false)
    {
        if (speechText.text != speech)
        {
            if (!needReWrite)
                speechWriter.WriteOnNewText = false;

            speechText.text = speech;

            if (!needReWrite)
                speechWriter.WriteOnNewText = true;
        }
    }

    public void DisplaySpeech(string text, bool alwaysDisplay = false)
    {
        hideSequence.StopSequence();
        appearSequence.StartSequence();
        speechText.text = text;
        isSpeechDisplayed = true;
        speechTimerEnd = Time.time + speechDuration;
        speechAlwaysDisplay = alwaysDisplay;
    }

    public void DisplaySpeech(string text, float duration)
    {
        hideSequence.StopSequence();
        appearSequence.StartSequence();
        speechText.text = text;
        isSpeechDisplayed = true;
        speechTimerEnd = Time.time + duration;
        speechAlwaysDisplay = false;
    }

    public void StopSpeech()
    {
        appearSequence.StopSequence();
        hideSequence.StartSequence();
        speechText.text = "";
        isSpeechDisplayed = false;
    }
}
