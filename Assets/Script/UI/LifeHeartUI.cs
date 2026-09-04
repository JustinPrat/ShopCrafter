using UnityEngine;

public class LifeHeartUI : MonoBehaviour
{
    [SerializeField]
    Sequencer.Sequencer activateSequence;

    [SerializeField]
    Sequencer.Sequencer deactivateSequence;

    private bool activated = true;
    public bool Activated => activated;

    public void Activate()
    {
        activated = true;
        if (activateSequence != null)
            activateSequence.StartSequence();
    }

    public void Deactivate()
    {
        activated = false;
        if (deactivateSequence != null)
            deactivateSequence.StartSequence();
    }
}
