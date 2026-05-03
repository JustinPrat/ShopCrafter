using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReceiver : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private PlayerInput playerInput;

    private InputSystem_Actions action;
    private bool isGamepad;

    public InputSystem_Actions Actions => action;
    public bool IsGamepad => isGamepad;
    public Action OnInputDeviceChanged;

    private void Awake()
    {
        action = new InputSystem_Actions();
        action.Enable();
        playerInput.actions = action.asset;

        managerRefs.InputManager = this;

        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == null)
            return;

        bool tmpIsGamepad = input.currentControlScheme.Equals("Gamepad") ? true : false;

        if (isGamepad != tmpIsGamepad)
        {
            isGamepad = tmpIsGamepad;
            OnInputDeviceChanged?.Invoke();

            Debug.Log("Input Device Changed: " + (isGamepad ? "GamePad" : "Keyboard/Mouse"));
        }
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
