using TriInspector;
using UnityEngine;

public class CraftedObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string interactText;

    [ReadOnly, SerializeField]
    private CraftedObjectData craftedObjectData;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public bool IsLocked { get; set; }
    public int Price => craftedObjectData.GetPrice();
    public CraftedObjectData CraftedData => craftedObjectData;
    public Collider Collider => collider;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;

    public void Init (CraftedObjectData data)
    {
        craftedObjectData = data;
        spriteRenderer.sprite = data.CraftedObjectRecipe.CraftedSprite;
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
