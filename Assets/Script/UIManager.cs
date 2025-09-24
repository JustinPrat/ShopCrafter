using System.Collections;
using System.Collections.Generic;
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
    private Canvas canvas;

    private CraftingView craftingViewInstance;
    private MiniGameView miniGameViewInstance;
    private DialogueView dialogueViewInstance;
    private ShopView shopViewInstance;
    private CraftedStatView craftedStatViewInstance;
    private EncyclopedieView encyclopedieViewInstance;

    public DialogueView DialogueView => dialogueViewInstance;

    private void Awake()
    {
        managerRefs.UIManager = this;
        craftingViewInstance = Instantiate(craftingViewPrefab).GetComponent<CraftingView>();
        craftingViewInstance.gameObject.SetActive(false);

        miniGameViewInstance = Instantiate(miniGameViewPrefab).GetComponent<MiniGameView>();
        miniGameViewInstance.gameObject.SetActive(false);

        dialogueViewInstance = Instantiate(dialogueViewPrefab, canvas.transform).GetComponent<DialogueView>();
        dialogueViewInstance.gameObject.SetActive(false);

        shopViewInstance = Instantiate(shopViewPrefab, canvas.transform).GetComponent<ShopView>();
        shopViewInstance.gameObject.SetActive(false);

        craftedStatViewInstance = Instantiate(craftedStatViewPrefab).GetComponent<CraftedStatView>();
        craftedStatViewInstance.gameObject.SetActive(false);

        encyclopedieViewInstance = Instantiate(encyclopedieViewPrefab, canvas.transform).GetComponent<EncyclopedieView>();
        encyclopedieViewInstance.gameObject.SetActive(false);

        managerRefs.InputManager.Actions.UI.Validate.Disable();
        managerRefs.InputManager.Actions.UI.Remove.Disable();
    }

    public void ToggleEncyclopedieView(bool isOn)
    {
        ExecuteAfterOneFrame(() =>
        {
            encyclopedieViewInstance.Toggle(isOn);

            if (isOn)
            {
                managerRefs.InputManager.SetActionType(false, false, false);
            }
            else
            {
                managerRefs.InputManager.SetActionType(true, true, true);
            }
        });
    }

    public void ToggleCraftingView (bool isOn, CraftingTable craftingTable, Vector3 pos = new Vector3())
    {
        ExecuteAfterOneFrame(() =>
        {
            craftingViewInstance.CurrentCraftingTable = craftingTable;
            craftingViewInstance.Toggle(isOn);
            craftingViewInstance.transform.position = pos + 1f * Vector3.up;

            if (isOn)
            {
                managerRefs.InputManager.SetActionType(false, false, true);
                managerRefs.InputManager.Actions.UI.Validate.Enable();
                managerRefs.InputManager.Actions.UI.Remove.Enable();
            }
            else
            {
                managerRefs.InputManager.SetActionType(true, true, true);
                managerRefs.InputManager.Actions.UI.Validate.Disable();
                managerRefs.InputManager.Actions.UI.Remove.Disable();
            }
        });
    }

    public void ToggleMiniGameView (bool isOn, CraftingTable craftingTable, Vector3 pos = new Vector3())
    {
        ExecuteAfterOneFrame(() =>
        {
            miniGameViewInstance.CurrentCraftingTable = craftingTable;
            miniGameViewInstance.Toggle(isOn);
            miniGameViewInstance.transform.position = pos + 1f * Vector3.up;

            if (isOn)
            {
                managerRefs.InputManager.SetActionType(false, false, true);
            }
            else
            {
                managerRefs.InputManager.SetActionType(true, true, true);
            }
        });
    }

    public void ToggleDialogueView (bool isOn, DialogueData firstData = null, PNJBehaviour pnjBehaviour = null)
    {
        ExecuteAfterOneFrame(() =>
        {
            dialogueViewInstance.Toggle(isOn);
            if (isOn)
            {
                dialogueViewInstance.Setup(firstData, pnjBehaviour);
                managerRefs.InputManager.SetActionType(false, false, true);
                managerRefs.InputManager.Actions.UI.Validate.Enable();
            }
            else
            {
                managerRefs.InputManager.SetActionType(true, true, true);
                managerRefs.InputManager.Actions.UI.Validate.Disable();
            }
        });
    }

    public void ToggleShopView (bool isOn, List<SellingItem> sellingItems = null, PNJBehaviour pnjBehaviour = null)
    {
        ExecuteAfterOneFrame(() =>
        {
            shopViewInstance.Toggle(isOn);
            if (isOn)
            {
                shopViewInstance.Setup(sellingItems, pnjBehaviour);
                managerRefs.InputManager.Actions.Player.Disable();
                managerRefs.InputManager.Actions.UI.Validate.Enable();
                managerRefs.InputManager.Actions.UI.Remove.Enable();
            }
            else
            {
                managerRefs.InputManager.Actions.Player.Enable();
                managerRefs.InputManager.Actions.UI.Validate.Disable();
                managerRefs.InputManager.Actions.UI.Remove.Disable();
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
