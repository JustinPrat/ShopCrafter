using System;
using UnityEngine;
using UnityEngine.Events;

public class ToggleActivation : MonoBehaviour, IInteractable
{
    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    [SerializeField]
    protected bool activeState;

    [SerializeField]
    protected Collider physicCollider;

    public bool ActiveState => activeState;

    public bool IsLocked { get; set; }

    public string InteractText => "Toggle";

    public GameObject GameObject => gameObject;

    public Collider PhysicCollider => physicCollider;

    public Action<IInteractable> OnDestroyEvent { get; set; }


    private void Start()
    {
        CallEvents();
    }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public void Toggle()
    {
        activeState = !activeState;
        CallEvents();
    }

    public void SetState(bool active)
    {
        activeState = active;
        CallEvents();
    }

    private void CallEvents()
    {
        if (activeState)
        {
            OnToggleOn?.Invoke();
        }
        else
        {
            OnToggleOff?.Invoke();
        }
    }

    public virtual bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public virtual void DoInteract(PlayerBrain playerBrain)
    {
        Toggle();
    }

    public virtual void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public virtual void OutOfInteractRange(PlayerBrain playerBrain)
    {
    }
}
