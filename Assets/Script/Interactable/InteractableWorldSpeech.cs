using Alchemy.Inspector;
using System;
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

    private bool isInteracting;

    private void Awake()
    {
        interactable.Value.OnTargetedEvent += OnTargetedEvent;
        interactable.Value.OnUnTargetedEvent += OnUnTargetedEvent;

        if (textUpdateWhenInteracting)
        {
            interactable.Value.OnStartInteractEvent += OnStartInteract;
            interactable.Value.OnReleaseInteractEvent += OnReleaseInteract;
        }
    }

    private void OnDestroy()
    {
        interactable.Value.OnTargetedEvent -= OnTargetedEvent;
        interactable.Value.OnUnTargetedEvent -= OnUnTargetedEvent;

        if (textUpdateWhenInteracting)
        {
            interactable.Value.OnStartInteractEvent -= OnStartInteract;
            interactable.Value.OnReleaseInteractEvent -= OnReleaseInteract;
        }
    }

    private void Update()
    {
        if (isInteracting)
        {
            worldSpeech.UpdateSpeech(interactable.Value.InteractText);
        }
    }

    private void OnTargetedEvent(IInteractable interactable)
    {
        worldSpeech.DisplaySpeech(interactable.InteractText, true);
    }

    private void OnUnTargetedEvent(IInteractable interactable)
    {
        worldSpeech.StopSpeech();
        isInteracting = false;
    }

    private void OnStartInteract(IInteractable interactable)
    {
        isInteracting = true;
    }

    private void OnReleaseInteract(IInteractable interactable)
    {
        isInteracting = false;
    }
}
