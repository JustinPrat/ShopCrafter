using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private Transform objectHoldAnchor;

    public Transform ObjectHoldAnchor => objectHoldAnchor;

    public Vector2 LastPlayerMovement { get; set; }

    public CraftedObject HeldObject { get; set; }

    public bool HasItem => HeldObject != null;

    public void TryHoldItem (CraftedObject craftedObject)
    {
        if (!HasItem)
        {
            HeldObject = craftedObject;
        }
    }

    public void DropItem ()
    {
        HeldObject = null;
    }
}
