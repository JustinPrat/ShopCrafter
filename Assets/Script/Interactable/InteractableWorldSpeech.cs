using TNRD;
using UnityEngine;

public class InteractableWorldSpeech : MonoBehaviour
{
    [SerializeField]
    private SerializableInterface<IInteractable> interactable;

    [SerializeField]
    private WorldSpeech worldSpeech;

    [SerializeField]
    private bool textUpdateWhenInteracting;

    private bool isHoldingInteract;
    private bool isTargeted;

    private void Awake()
    {
        interactable.Value.OnTargetedEvent += OnTargetedEvent;
        interactable.Value.OnUnTargetedEvent += OnUnTargetedEvent;
        interactable.Value.OnStartInteractEvent += OnStartInteract;
        interactable.Value.OnReleaseInteractEvent += OnReleaseInteract;
    }

    private void OnDestroy()
    {
        interactable.Value.OnTargetedEvent -= OnTargetedEvent;
        interactable.Value.OnUnTargetedEvent -= OnUnTargetedEvent;
        interactable.Value.OnStartInteractEvent -= OnStartInteract;
        interactable.Value.OnReleaseInteractEvent -= OnReleaseInteract;
    }

    private void Update()
    {
        if (isHoldingInteract)
        {
            worldSpeech.UpdateSpeech(interactable.Value.InteractText);
        }

        //Interaction ON and not interacting = ReEnable speech
        if (!worldSpeech.IsSpeechDisplayed && !interactable.Value.IsInteracting && isTargeted)
        {
            worldSpeech.DisplaySpeech(interactable.Value.InteractText, true);
        }
    }

    private void OnTargetedEvent(IInteractable interactable)
    {
        isTargeted = true;
        worldSpeech.DisplaySpeech(interactable.InteractText, true);
    }

    private void OnUnTargetedEvent(IInteractable interactable)
    {
        worldSpeech.StopSpeech();
        isHoldingInteract = false;
        isTargeted = false;
    }

    private void OnStartInteract(IInteractable interactable)
    {
        if (textUpdateWhenInteracting)
            isHoldingInteract = true;
        else
            worldSpeech.StopSpeech();
    }

    private void OnReleaseInteract(IInteractable interactable)
    {
        if (textUpdateWhenInteracting)
            isHoldingInteract = false;
    }
}
