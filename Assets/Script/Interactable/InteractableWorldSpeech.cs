using TNRD;
using UnityEngine;

public class InteractableWorldSpeech : MonoBehaviour
{
    [SerializeField]
    private SerializableInterface<IInteractable> interactable;

    [SerializeField]
    private WorldSpeech worldSpeech;

    private void Awake()
    {
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
        worldSpeech.DisplaySpeech(interactable.InteractText, true);
    }

    private void OnUnTargetedEvent(IInteractable interactable)
    {
        worldSpeech.StopSpeech();
    }
}
