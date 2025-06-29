using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private Transform objectHoldAnchor;

    public Transform ObjectHoldAnchor => objectHoldAnchor;

    public Vector2 LastPlayerMovement { get; set; }

    public CraftedObject HeldObject { get; set; }

    public void TryHoldItem (CraftedObject craftedObject)
    {
        if (CanHoldItem())
        {
            HeldObject = craftedObject;
        }
    }

    public bool CanHoldItem ()
    {
        return HeldObject == null;
    }
}
