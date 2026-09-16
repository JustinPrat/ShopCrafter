using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PickBox : MonoBehaviour, IInteractable
{
    [SerializeField]
    private List<GameObject> spawnObjectList;

    [SerializeField]
    private Collider physicCollider;

    [SerializeField]
    private CinemachineImpulseSource cinemachineImpulseSource;

    public bool IsLocked { get; set; }

    public string InteractText => "Open";

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }
    public Action<IInteractable> OnTargetedEvent { get; set; }
    public Action<IInteractable> OnUnTargetedEvent { get; set; }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        cinemachineImpulseSource.GenerateImpulse(0.2f);
        Destroy(gameObject);
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
        OnTargetedEvent?.Invoke(this);
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
        OnUnTargetedEvent?.Invoke(this);
    }
}
