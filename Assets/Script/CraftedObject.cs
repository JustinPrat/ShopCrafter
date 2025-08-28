using Alchemy.Inspector;
using UnityEngine;

public class CraftedObject : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private Sprite icon;

    [ReadOnly, SerializeField]
    private CraftedObjectData craftedObjectData;

    Sprite IInteractable.InteractIcon => icon;
    public int Price => craftedObjectData.GetPrice();

    public void Init (CraftedObjectData data)
    {
        craftedObjectData = data;
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return false;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        //transform.SetParent(playerBrain.ObjectHoldAnchor);
        //transform.localPosition = Vector3.zero;
        //playerBrain.TryHoldItem(this);
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
    }
}
