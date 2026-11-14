using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReceiver : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private PlayerInput playerInput;

    public InputSystem_Actions Actions => actions;
    public bool IsGamepad => isGamepad;
    public Action OnInputDeviceChanged;

    private Dictionary<PlayerInputType, InputAction> playerInputActionMap;
    private Dictionary<UIInputType, InputAction> UIInputActionMap;
    private InputSystem_Actions actions;
    private bool isGamepad;

    private void Awake()
    {
        actions = new InputSystem_Actions();
        actions.Enable();
        playerInput.actions = actions.asset;

        playerInputActionMap = new Dictionary<PlayerInputType, InputAction>
        {
            { PlayerInputType.Move, actions.Player.Move },
            { PlayerInputType.Interact, actions.Player.Move }
        };

        UIInputActionMap = new Dictionary<UIInputType, InputAction>
        {
            { UIInputType.Submit, actions.UI.Submit }
        };

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
        SetAction(actions.Player.Move, canMove);
        SetAction(actions.Player.Interact, canInteract);
        SetAction(actions.UI.Submit, canNextDialogue);
    }

    private void SetAction (InputAction inputAction, bool newState)
    {
        if (newState)
            inputAction.Enable();
        else
            inputAction.Disable();
    }

    public void SetAction(PlayerInputType playerInputType, bool newState)
    {
        foreach (KeyValuePair<PlayerInputType, InputAction> inputTypeAction in playerInputActionMap)
        {
            if ((playerInputType & inputTypeAction.Key) != 0)
            {
                SetAction(inputTypeAction.Value, newState);
            }
        }
    }

    public void SetAction(UIInputType UIInputType, bool newState)
    {
        foreach (KeyValuePair<UIInputType, InputAction> inputTypeAction in UIInputActionMap)
        {
            if ((UIInputType & inputTypeAction.Key) != 0)
            {
                SetAction(inputTypeAction.Value, newState);
            }
        }
    }
}

[Flags]
public enum PlayerInputType
{
    Move,
    Interact
}

[Flags]
public enum UIInputType
{
    Submit
}
