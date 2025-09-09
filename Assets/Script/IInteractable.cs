using UnityEngine;

public interface IInteractable
{
    public Sprite InteractIcon { get;  }

    public void DoInteract(PlayerBrain playerBrain);

    public bool CanInteract(PlayerBrain playerBrain);

    public void OutOfInteractRange(PlayerBrain playerBrain);

    public void OnInteractRange(PlayerBrain playerBrain);
}
