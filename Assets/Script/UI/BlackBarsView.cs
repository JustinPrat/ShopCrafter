using UnityEngine;

public class BlackBarsView : UIView
{
    [SerializeField]
    private Sequencer.Sequencer outSequencer;

    protected override void SetVisualActivationView(bool isOn)
    {
        if (isOn)
        {
            gameObject.SetActive(true);
        }
        else
        {
            outSequencer.StartSequence();
        }
    }
}
