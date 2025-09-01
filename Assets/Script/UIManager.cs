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
    private Canvas canvas;

    private CraftingView craftingViewInstance;
    private MiniGameView miniGameViewInstance;
    private DialogueView dialogueViewInstance;
    private ShopView shopViewInstance;

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
    }

    public void ToggleCraftingView (bool isOn, CraftingTable craftingTable, Vector3 pos = new Vector3())
    {
        craftingViewInstance.CurrentCraftingTable = craftingTable;
        craftingViewInstance.Toggle(isOn);
        craftingViewInstance.transform.position = pos + 1f * Vector3.up;
    }

    public void ToggleMiniGameView (bool isOn, CraftingTable craftingTable, Vector3 pos = new Vector3())
    {
        miniGameViewInstance.CurrentCraftingTable = craftingTable;
        miniGameViewInstance.Toggle(isOn);
        miniGameViewInstance.transform.position = pos + 1f * Vector3.up;
    }

    public void ToggleDialogueView (bool isOn, DialogueData firstData = null, PNJBehaviour pnjBehaviour = null)
    {
        dialogueViewInstance.Toggle(isOn);
        if (isOn)
        {
            dialogueViewInstance.Setup(firstData, pnjBehaviour);
        }
    }

    public void ToggleShopView (bool isOn, List<SellingItem> sellingItems = null, PNJBehaviour pnjBehaviour = null)
    {
        shopViewInstance.Toggle(isOn);
        if (isOn)
        {
            shopViewInstance.Setup(sellingItems, pnjBehaviour);
        }
    }
}
