using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MaterialInventoryView : UIView
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private Transform scrollMaterials;

    [SerializeField]
    private GameObject itemMaterialUIPrefab;

    private void Awake()
    {
        managerRefs.InputManager.Actions.UI.Cancel.started += OnCancel;
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.UI.Cancel.started -= OnCancel;
    }

    private void OnCancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (gameObject.activeInHierarchy)
        {
            CloseShop();
        }
    }

    public void CloseShop() // inspector button click
    {
        managerRefs.UIManager.ToggleMaterialInventoryView(false);
    }

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);

        if (isOn)
        {
            foreach (KeyValuePair<Item, int> itemCount in managerRefs.CraftingManager.ItemInventory)
            {
                ItemInventoryUI itemUI = Instantiate(itemMaterialUIPrefab, scrollMaterials).GetComponent<ItemInventoryUI>();
                itemUI.Setup(itemCount.Key, itemCount.Value);
            }
        }
        else
        {
            foreach (Transform child in scrollMaterials)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
