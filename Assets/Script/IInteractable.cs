using UnityEngine;

public interface IInteractable
{
    public Sprite Icon { get;  }

    public void DoInteract(PlayerBrain playerBrain);

    public bool CanInteract(PlayerBrain playerBrain);
}
