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
    private Movement movement;

    [SerializeField]
    private Interaction interaction;

    public Inventory Inventory => inventory;
    public Movement Movement => movement;
    public Interaction Interaction => interaction;
    public Vector2 LastPlayerMovement { get; set; }
    public ManagerRefs ManagerRefsProperty => managerRefs;

    private void Awake()
    {
    }
}
