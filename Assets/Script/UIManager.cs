using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject craftingView;

    [SerializeField]
    private GameObject miniGameView;

    private UIView craftingViewInstance;
    private UIView miniGameViewInstance;

    private void Awake()
    {
        managerRefs.UIManager = this;
        craftingViewInstance = Instantiate(craftingView).GetComponent<UIView>();
        craftingViewInstance.gameObject.SetActive(false);

        miniGameViewInstance = Instantiate(miniGameView).GetComponent<UIView>();
        miniGameViewInstance.gameObject.SetActive(false);
    }

    public void ToggleCraftingView (bool isOn, Vector3 pos = new Vector3())
    {
        craftingViewInstance.Toggle(isOn);
        craftingViewInstance.transform.position = pos + 1f * Vector3.up;
    }

    public void ToggleMiniGameView (bool isOn, Vector3 pos = new Vector3())
    {
        miniGameViewInstance.Toggle(isOn);
        miniGameViewInstance.transform.position = pos + 1f * Vector3.up;
    }
}
