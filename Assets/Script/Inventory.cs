using System;
using System.Collections;
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

    [SerializeField]
    private GameObject itemPrefab;

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
        managerRefs.UIManager.ToggleInventoryUI(true);
        StartCoroutine(InitUI());
    }

    IEnumerator InitUI()
    {
        yield return null;
        managerRefs.GameEventsManager.playerEvents.UpdateInventory(craftedInventory);
        managerRefs.GameEventsManager.playerEvents.SelectedInventoryIndexChange(selectedInventoryIndex);
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.Player.Inventory.performed -= OnSelectInventoryPerformed;
    }

    private void OnSelectInventoryPerformed(InputAction.CallbackContext context)
    {
        int direction = (int)context.ReadValue<float>();
        if (selectedInventoryIndex + direction < inventorySpace && selectedInventoryIndex + direction >= 0)
        {
            selectedInventoryIndex += direction;
            OnChangeIndex();
            managerRefs.GameEventsManager.playerEvents.SelectedInventoryIndexChange(selectedInventoryIndex);
        }
    }

    private void OnChangeIndex()
    {
        if (HeldObject != null)
        {
            Destroy(HeldObject.gameObject);
            HeldObject = null;
        }

        if (craftedInventory[selectedInventoryIndex] != null)
        {
            GameObject newItem = Instantiate(itemPrefab, objectHoldAnchor.position, Quaternion.identity, objectHoldAnchor);
            HeldObject = newItem.GetComponent<CraftedObject>(); 
            HeldObject.Init(craftedInventory[selectedInventoryIndex]);
        }
    }

    public void DropItem()
    {
        if (HeldObject != null)
        {
            RemoveItemData(HeldObject.CraftedData);
        }
        HeldObject = null;
        managerRefs.GameEventsManager.playerEvents.UpdateInventory(craftedInventory);
    }

    public bool TryTakeItem(CraftedObject craftedObject)
    {
        int index = selectedInventoryIndex;
        if (craftedInventory[selectedInventoryIndex] == null || HasEmptySpace(out index))
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

    private void RemoveItemData (CraftedObjectData craftedObjectData)
    {
        foreach (KeyValuePair<int, CraftedObjectData> craftedIndexObject in craftedInventory)
        {
            if (craftedIndexObject.Value == craftedObjectData)
            {
                craftedInventory[craftedIndexObject.Key] = null; 
                return;
            }
        }
    }
}
