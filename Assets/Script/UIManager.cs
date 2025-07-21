using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject craftingView;

    private UIView craftingViewInstance;

    private void Awake()
    {
        managerRefs.UIManager = this;
        craftingViewInstance = Instantiate(craftingView).GetComponent<UIView>();
        craftingViewInstance.gameObject.SetActive(false);
    }

    public void ToggleCraftingView (bool isOn, Vector3 pos)
    {
        craftingViewInstance.Toggle(isOn);
        craftingViewInstance.transform.position = pos + 1f * Vector3.up;
    }
}
