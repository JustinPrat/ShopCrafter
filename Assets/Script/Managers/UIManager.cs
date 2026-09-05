using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject craftingViewPrefab;

    [SerializeField]
    private GameObject miniGameViewPrefab;

    [SerializeField]
    private GameObject dialogueViewPrefab;

    [SerializeField]
    private GameObject shopViewPrefab;

    [SerializeField]
    private GameObject craftedStatViewPrefab;

    [SerializeField]
    private GameObject encyclopedieViewPrefab;

    [SerializeField]
    private GameObject priceCheckViewPrefab;

    [SerializeField]
    private GameObject inventoryViewPrefab;

    [SerializeField]
    private GameObject endDayViewPrefab;

    [SerializeField]
    private GameObject startDayViewPrefab;

    [SerializeField]
    private GameObject inventoryMaterialViewPrefab;

    [SerializeField]
    private GameObject cardTagViewPrefab;

    [SerializeField]
    private GameObject rewardViewPrefab;

    [SerializeField]
    private Canvas canvas;

    private CraftingView craftingViewInstance;
    private MiniGameView miniGameViewInstance;
    private CardTagView cardTagViewInstance;
    private DialogueView dialogueViewInstance;
    private ShopView shopViewInstance;
    private CraftedStatView craftedStatViewInstance;
    private EncyclopedieView encyclopedieViewInstance;
    private PriceCheckView priceCheckViewInstance;
    private InventoryView inventoryViewInstance;
    private EndDayView endDayViewInstance;
    private StartDayView startDayViewInstance;
    private MaterialInventoryView materialInventoryViewInstance;
    private RewardView rewardViewInstance;

    private List<UIView> focusList = new List<UIView>();
    public UIView CurrentFocus => focusList.Last();

    public DialogueView DialogueView => dialogueViewInstance;

    private void Awake()
    {
        managerRefs.UIManager = this;
        craftingViewInstance = Instantiate(craftingViewPrefab).GetComponent<CraftingView>();
        craftingViewInstance.gameObject.SetActive(false);

        miniGameViewInstance = Instantiate(miniGameViewPrefab).GetComponent<MiniGameView>();
        miniGameViewInstance.gameObject.SetActive(false);

        cardTagViewInstance = Instantiate(cardTagViewPrefab, canvas.transform).GetComponent<CardTagView>();
        cardTagViewInstance.gameObject.SetActive(false);

        inventoryViewInstance = Instantiate(inventoryViewPrefab, canvas.transform).GetComponent<InventoryView>();
        inventoryViewInstance.gameObject.SetActive(false);

        dialogueViewInstance = Instantiate(dialogueViewPrefab, canvas.transform).GetComponent<DialogueView>();
        dialogueViewInstance.gameObject.SetActive(false);

        shopViewInstance = Instantiate(shopViewPrefab, canvas.transform).GetComponent<ShopView>();
        shopViewInstance.gameObject.SetActive(false);

        craftedStatViewInstance = Instantiate(craftedStatViewPrefab).GetComponent<CraftedStatView>();
        craftedStatViewInstance.gameObject.SetActive(false);

        encyclopedieViewInstance = Instantiate(encyclopedieViewPrefab, canvas.transform).GetComponent<EncyclopedieView>();
        encyclopedieViewInstance.gameObject.SetActive(false);

        priceCheckViewInstance = Instantiate(priceCheckViewPrefab).GetComponent<PriceCheckView>();
        priceCheckViewInstance.gameObject.SetActive(false);

        endDayViewInstance = Instantiate(endDayViewPrefab, canvas.transform).GetComponent<EndDayView>();
        endDayViewInstance.gameObject.SetActive(false);

        startDayViewInstance = Instantiate(startDayViewPrefab, canvas.transform).GetComponent<StartDayView>();
        startDayViewInstance.gameObject.SetActive(false);

        materialInventoryViewInstance = Instantiate(inventoryMaterialViewPrefab, canvas.transform).GetComponent<MaterialInventoryView>();
        materialInventoryViewInstance.gameObject.SetActive(false);

        rewardViewInstance = Instantiate(rewardViewPrefab, canvas.transform).GetComponent<RewardView>();
        rewardViewInstance.gameObject.SetActive(false);

        managerRefs.InputManager.Actions.UI.Submit.Disable();
        managerRefs.InputManager.Actions.UI.Cancel.Disable();

        managerRefs.InputManager.Actions.Player.MaterialInventory.performed += InventoryPerformed;
    }

    private void NewFocus(UIView newFocus)
    {
        focusList.Add(newFocus);
    }

    private void RemoveFocus(UIView newFocus)
    {
        if (focusList.Contains(newFocus))
        {
            focusList.Remove(newFocus);
        }
    }

    private void UpdateFocus(bool isOn, UIView uiView)
    {
        if (isOn)
            NewFocus(uiView);
        else
            RemoveFocus(uiView);
    }

    private void OnDestroy()
    {
        if (managerRefs.InputManager != null)
        {
            managerRefs.InputManager.Actions.Player.MaterialInventory.performed -= InventoryPerformed;
        }
    }

    private void InventoryPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        ToggleMaterialInventoryView(!materialInventoryViewInstance.gameObject.activeInHierarchy);
    }

    public void ToggleInventoryUI (bool isOn)
    {
        ExecuteAfterOneFrame(() =>
        {
            inventoryViewInstance.Toggle(isOn);
        });
    }

    public void ToggleMaterialInventoryView(bool isOn)
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, materialInventoryViewInstance);
            materialInventoryViewInstance.Toggle(isOn);
        });
    }

    public void ToggleRewardView(bool isOn, IRewardable rewardable = null)
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, rewardViewInstance);
            rewardViewInstance.Toggle(isOn);

            if (isOn)
            {
                rewardViewInstance.Setup(rewardable);
                Time.timeScale = 0f;
                managerRefs.InputManager.SetActionType(false, false, true);
            }
            else
            {
                Time.timeScale = 1f;
                managerRefs.InputManager.SetActionType(true, true, true);
            }
        });
    }
    
    public void ToggleEndDayView(bool isOn)
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, endDayViewInstance);
            endDayViewInstance.Toggle(isOn);

            if (isOn)
            {
                Time.timeScale = 0f;
                managerRefs.InputManager.SetActionType(false, false, true);
                managerRefs.InputManager.Actions.UI.Submit.Enable();
            }
            else
            {
                Time.timeScale = 1f;
                managerRefs.InputManager.SetActionType(true, true, true);
                managerRefs.InputManager.Actions.UI.Submit.Disable();
            }
        });
    }

    public void ToggleStartDayView(bool isOn)
    {
        ExecuteAfterOneFrame(() =>
        {
            startDayViewInstance.Toggle(isOn);
        });
    }

    public void ToggleEncyclopedieView(bool isOn)
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, encyclopedieViewInstance);
            encyclopedieViewInstance.Toggle(isOn);

            if (isOn)
            {
                managerRefs.InputManager.SetActionType(false, false, true);
                managerRefs.InputManager.Actions.UI.Submit.Enable();
                managerRefs.InputManager.Actions.UI.Cancel.Enable();
            }
            else
            {
                managerRefs.InputManager.SetActionType(true, true, true);
                managerRefs.InputManager.Actions.UI.Submit.Disable();
                managerRefs.InputManager.Actions.UI.Cancel.Disable();
            }
        });
    }

    public void ToggleCraftingView (bool isOn, CraftingTable craftingTable, Vector3 pos = new Vector3())
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, craftingViewInstance);
            craftingViewInstance.CurrentCraftingTable = craftingTable;
            craftingViewInstance.Toggle(isOn);
            craftingViewInstance.transform.position = pos;

            if (isOn)
            {
                managerRefs.InputManager.SetActionType(false, false, true);
                managerRefs.InputManager.Actions.UI.Submit.Enable();
                managerRefs.InputManager.Actions.UI.Cancel.Enable();
            }
            else
            {
                managerRefs.InputManager.SetActionType(true, true, true);
                managerRefs.InputManager.Actions.UI.Submit.Disable();
                managerRefs.InputManager.Actions.UI.Cancel.Disable();
            }
        });
    }

    public void ShowMiniGameView(CraftingTable craftingTable, CraftedObjectData data, Vector3 pos = new Vector3())
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(true, miniGameViewInstance);
            miniGameViewInstance.CurrentCraftingTable = craftingTable;
            miniGameViewInstance.Setup(data);
            miniGameViewInstance.Toggle(true);

            miniGameViewInstance.transform.position = pos + 1f * Vector3.up;

            managerRefs.InputManager.SetActionType(false, false, true);
            managerRefs.InputManager.Actions.UI.Submit.Enable();
            managerRefs.InputManager.Actions.UI.Cancel.Enable();
        });
    }

    public void HideMiniGameView()
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(false, miniGameViewInstance);
            miniGameViewInstance.Toggle(false);
            managerRefs.InputManager.SetActionType(true, true, true);
            managerRefs.InputManager.Actions.UI.Submit.Disable();
            managerRefs.InputManager.Actions.UI.Cancel.Disable();
        });
    }

    public void ToggleCardTagView(bool isOn, CraftingTable craftingTable, List<Item> consumedItems = null)
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, cardTagViewInstance);
            cardTagViewInstance.CurrentCraftingTable = craftingTable;
            cardTagViewInstance.Toggle(isOn);

            if (isOn)
            {
                Time.timeScale = 0f;
                cardTagViewInstance.Setup(consumedItems);
                managerRefs.InputManager.SetActionType(false, false, true);
                managerRefs.InputManager.Actions.UI.Submit.Enable();
                managerRefs.InputManager.Actions.Player.Navigate.Enable();
            }
            else
            {
                Time.timeScale = 1f;
                managerRefs.InputManager.SetActionType(true, true, true);
                managerRefs.InputManager.Actions.UI.Submit.Disable();
            }
        });
    }

    public void ToggleDialoguePNJView (bool isOn, DialogueData firstData = null, PNJBrain pnjBrain = null)
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, dialogueViewInstance);
            dialogueViewInstance.Toggle(isOn);
            if (isOn)
            {
                Time.timeScale = 0f;
                dialogueViewInstance.Setup(firstData, pnjBrain);
                managerRefs.InputManager.SetActionType(false, false, true);
                managerRefs.InputManager.Actions.UI.Submit.Enable();
                managerRefs.InputManager.Actions.UI.Cancel.Enable();
            }
            else
            {
                Time.timeScale = 1f;
                managerRefs.InputManager.SetActionType(true, true, true);
                managerRefs.InputManager.Actions.UI.Submit.Disable();
                managerRefs.InputManager.Actions.UI.Cancel.Disable();
            }
        });
    }

    public void ToggleDialogueDataView(bool isOn, DialogueData firstData = null, Identity identity = null)
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, dialogueViewInstance);
            dialogueViewInstance.Toggle(isOn);
            if (isOn)
            {
                Time.timeScale = 0f;
                dialogueViewInstance.Setup(firstData, identity);
                managerRefs.InputManager.SetActionType(false, false, true);
                managerRefs.InputManager.Actions.UI.Submit.Enable();
            }
            else
            {
                Time.timeScale = 1f;
                managerRefs.InputManager.SetActionType(true, true, true);
                managerRefs.InputManager.Actions.UI.Submit.Disable();
            }
        });
    }

    public void ToggleShopView (bool isOn, List<SellingItem> sellingItems = null, SellerRuntime sellerTrait = null, PNJBrain pnjBrain = null)
    {
        ExecuteAfterOneFrame(() =>
        {
            UpdateFocus(isOn, shopViewInstance);
            shopViewInstance.Toggle(isOn);
            if (isOn)
            {
                shopViewInstance.Setup(sellingItems, sellerTrait, pnjBrain);
                managerRefs.InputManager.Actions.Player.Disable();
                managerRefs.InputManager.Actions.UI.Submit.Enable();
                managerRefs.InputManager.Actions.UI.Cancel.Enable();
            }
            else
            {
                managerRefs.InputManager.Actions.Player.Enable();
                managerRefs.InputManager.Actions.UI.Submit.Disable();
                managerRefs.InputManager.Actions.UI.Cancel.Disable();
            }
        });
    }

    public void ToggleCraftedStatView (bool isOn, CraftedObjectData craftedObjectData = null, Vector3 pos = new Vector3())
    {
        craftedStatViewInstance.Toggle(isOn);

        if (isOn)
        {
            craftedStatViewInstance.Setup(craftedObjectData, pos);
        }
    }

    public void TogglePriceCheckView (bool isOn, Vector3 pos = new Vector3())
    {
        priceCheckViewInstance.Toggle(isOn);

        if (isOn)
        {
            priceCheckViewInstance.Setup(pos);
        }
    }

    public void ExecuteAfterOneFrame(System.Action actionToExecute)
    {
        StartCoroutine(ExecuteActionAfterOneFrameCoroutine(actionToExecute));
    }

    private IEnumerator ExecuteActionAfterOneFrameCoroutine(System.Action actionToExecute)
    {
        yield return new WaitForEndOfFrame();
        actionToExecute?.Invoke();
    }
}
