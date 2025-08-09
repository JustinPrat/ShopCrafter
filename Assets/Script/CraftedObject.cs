using UnityEngine;

public class CraftedObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite icon;
    private int price;

    private CraftedObjectRecipe craftedObjectData;

    Sprite IInteractable.InteractIcon => icon;
    public int Price => price;

    public void Init (CraftedObjectRecipe data)
    {
        craftedObjectData = data;

        // evaluate price depending on the required items
    }

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

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
    }
}
