using UnityEngine;

public interface IInteractable
{
    public bool IsLocked { get; set; }
    //public Sprite InteractIcon { get;  }

    public string InteractText { get; }
    public GameObject GameObject { get; }
    public Collider PhysicCollider { get; }

    public void DoInteract(PlayerBrain playerBrain);

    public bool CanInteract(PlayerBrain playerBrain);

    public void OutOfInteractRange(PlayerBrain playerBrain);

    public void OnInteractRange(PlayerBrain playerBrain);
}
