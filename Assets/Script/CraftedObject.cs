using UnityEngine;

public class CraftedObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite icon;

    Sprite IInteractable.Icon => icon;

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return playerBrain.CanHoldItem();
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        transform.SetParent(playerBrain.ObjectHoldAnchor);
        transform.localPosition = Vector3.zero;
        playerBrain.TryHoldItem(this);
    }
}
