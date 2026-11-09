using System;
using UnityEngine;

public interface IInteractable
{
    public bool IsLocked { get; set; }

    public string InteractText { get; }
    public GameObject GameObject { get; }
    public Collider PhysicCollider { get; }
    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }
    public Action<IInteractable> OnStartInteractEvent { get; set; }
    public Action<IInteractable> OnReleaseInteractEvent { get; set; }

    public void StartInteract(PlayerBrain playerBrain);
    public void ReleaseInteract(PlayerBrain playerBrain);

    public bool CanInteract(PlayerBrain playerBrain);

    public void UnTargeted(PlayerBrain playerBrain);

    public void OnTargeted(PlayerBrain playerBrain);

    public void OnInteractRange(PlayerBrain playerBrain);
    public void OutInteractRange(PlayerBrain playerBrain);
}
