using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Transform objectHoldAnchor;

    [SerializeField]
    private int inventorySpace = 5;

    [SerializeField]
    private ManagerRefs managerRefs;

    private Dictionary<int, CraftedObjectData> craftedInventory = new Dictionary<int, CraftedObjectData>();
    private int selectedInventoryIndex;

    public CraftedObject HeldObject { get; set; }
    public bool HasItem => HeldObject != null;
    public bool HasEnoughInventorySpace => craftedInventory.Count < inventorySpace;
    public Transform ObjectHoldAnchor => objectHoldAnchor;

    private void Awake()
    {
        for (int i = 0; i < inventorySpace; i++)
        {
            craftedInventory.Add(i, null);
        }
    }

    private void Start()
    {
        managerRefs.InputManager.Actions.Player.Inventory.performed += OnSelectInventoryPerformed;
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.Player.Inventory.performed -= OnSelectInventoryPerformed;
    }

    private void OnSelectInventoryPerformed(InputAction.CallbackContext context)
    {
        // left right inventory selection
    }

    public bool TryTakeItem(CraftedObject craftedObject)
    {
        if (HasEmptySpace(out int index))
        {
            craftedInventory[index] = craftedObject.CraftedData;

            if (selectedInventoryIndex == index)
            {
                HeldObject = craftedObject;
            }

            managerRefs.GameEventsManager.playerEvents.UpdateInventory(craftedInventory);
            return true;
        }

        return false;
    }

    private bool HasEmptySpace(out int index)
    {
        for (int i = 0; i < inventorySpace; i++)
        {
            if (craftedInventory[i] == null)
            {
                index = i;
                return true;
            }
        }

        index = 0;
        return false;
    }

    public void DropItem()
    {
        HeldObject = null;
    }
}
