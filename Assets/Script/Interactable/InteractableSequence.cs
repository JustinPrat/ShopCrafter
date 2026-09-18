using TNRD;
using UnityEngine;

public class InteractableSequence : MonoBehaviour
{
    [SerializeField]
    private SerializableInterface<IInteractable> interactable;

    [SerializeField]
    private Sequencer.Sequencer onTargetSequencer;

    [SerializeField]
    private Sequencer.Sequencer onUntargetSequencer;

    private bool hasTargetSequencer;
    private bool hasUntargetSequencer;

    private void Awake()
    {
        hasTargetSequencer = onTargetSequencer != null;
        hasUntargetSequencer = onUntargetSequencer != null;

        interactable.Value.OnTargetedEvent += OnTargetedEvent;
        interactable.Value.OnUnTargetedEvent += OnUnTargetedEvent;
    }

    private void OnDestroy()
    {
        interactable.Value.OnTargetedEvent -= OnTargetedEvent;
        interactable.Value.OnUnTargetedEvent -= OnUnTargetedEvent;
    }

    private void OnTargetedEvent(IInteractable interactable)
    {
        if (hasUntargetSequencer)
            onUntargetSequencer.StopSequence();

        if (hasTargetSequencer)
            onTargetSequencer.StartSequence();
    }

    private void OnUnTargetedEvent(IInteractable interactable)
    {
        if (hasTargetSequencer)
            onTargetSequencer.StopSequence();

        if (hasUntargetSequencer)
            onUntargetSequencer.StartSequence();
    }
}
