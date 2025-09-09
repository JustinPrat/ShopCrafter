using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    private InputSystem_Actions action;

    public InputSystem_Actions Actions => action;

    private void Awake()
    {
        action = new InputSystem_Actions();
        action.Enable();

        managerRefs.InputManager = this;
    }
}
