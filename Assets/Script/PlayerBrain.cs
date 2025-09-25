using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    [SerializeField] 
    private Transform objectHoldAnchor;

    [SerializeField]
    private ManagerRefs managerRefs;
 
    public Transform ObjectHoldAnchor => objectHoldAnchor;

    public Vector2 LastPlayerMovement { get; set; }

    public CraftedObject HeldObject { get; set; }

    public bool HasItem => HeldObject != null;

    public ManagerRefs ManagerRefsProperty => managerRefs;

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
