using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    //[SerializeField] 
    //private Transform objectHoldAnchor;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private Interaction interaction;

    [SerializeField]
    private RotateWithDirection rotateWithDirection;

    private Vector2 lastPlayerMovement;

    public Inventory Inventory => inventory;
    public Interaction Interaction => interaction;
    public Vector2 LastPlayerMovement => lastPlayerMovement;
    public ManagerRefs ManagerRefsProperty => managerRefs;

    public void SetLastPlayerMovement(Vector2 movement)
    {
        lastPlayerMovement = movement;
        rotateWithDirection.UpdateWithRotation(movement);
    }

    public void StopMovementPlayer()
    {
        rotateWithDirection.StopMovement();
    }
}
