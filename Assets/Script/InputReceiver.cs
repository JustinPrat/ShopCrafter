using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void SetActionType (bool canMove = true, bool canInteract = true, bool canNextDialogue = true)
    {
        SetAction(managerRefs.InputManager.Actions.Player.Move, canMove);
        SetAction(managerRefs.InputManager.Actions.Player.Interact, canInteract);
        SetAction(managerRefs.InputManager.Actions.Player.NextDialogue, canNextDialogue);
    }

    private void SetAction (InputAction inputAction, bool newState)
    {
        if (newState)
            inputAction.Enable();
        else
            inputAction.Disable();
    }
}
