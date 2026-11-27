using UnityEngine;

public class TutoMiniGameSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject gamepadVersion;

    [SerializeField]
    private GameObject keyboardVersion;

    [SerializeField]
    private ManagerRefs managerRefs;

    private void Start()
    {
        OnDeviceChange();
        managerRefs.InputManager.OnInputDeviceChanged += OnDeviceChange;
    }

    private void OnDestroy()
    {
        if (managerRefs.InputManager != null)
            managerRefs.InputManager.OnInputDeviceChanged += OnDeviceChange;
    }

    private void OnDeviceChange()
    {
        if (managerRefs.InputManager.IsGamepad)
        {
            gamepadVersion.SetActive(true);
            keyboardVersion.SetActive(false);
        }
        else
        {
            gamepadVersion.SetActive(false);
            keyboardVersion.SetActive(true);
        }
    }
}
