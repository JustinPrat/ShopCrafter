using TriInspector;
using UnityEngine;

public class CraftedObject : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private Sprite icon;

    [ReadOnly, SerializeField]
    private CraftedObjectData craftedObjectData;

    [SerializeField]
    private Collider2D collider;

    Sprite IInteractable.InteractIcon => icon;
    public bool IsLocked { get; set; }
    public int Price => craftedObjectData.GetPrice();
    public CraftedObjectData CraftedData => craftedObjectData;
    public Collider2D Collider => collider;
    public GameObject GameObject => gameObject;

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

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }
}
