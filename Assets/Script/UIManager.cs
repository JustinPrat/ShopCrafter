using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject craftingView;

    [SerializeField]
    private GameObject miniGameView;

    private CraftingView craftingViewInstance;
    private MiniGameView miniGameViewInstance;

    private void Awake()
    {
        managerRefs.UIManager = this;
        craftingViewInstance = Instantiate(craftingView).GetComponent<CraftingView>();
        craftingViewInstance.gameObject.SetActive(false);

        miniGameViewInstance = Instantiate(miniGameView).GetComponent<MiniGameView>();
        miniGameViewInstance.gameObject.SetActive(false);
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
}
