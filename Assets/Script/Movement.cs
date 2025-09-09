using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] 
    private ControllerData controllerData;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private PlayerBrain playerBrain;

    private Vector2 lastInput;
    private Rigidbody2D rigidBody;

    private void Start()
    {
        managerRefs.InputManager.Actions.Player.Move.performed += OnMovePerformed;
        managerRefs.InputManager.Actions.Player.Move.canceled += OnMoveCanceled;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.Player.Move.performed -= OnMovePerformed;
        managerRefs.InputManager.Actions.Player.Move.canceled -= OnMoveCanceled;
    }

    private void Update()
    {
        rigidBody.linearVelocity = lastInput * controllerData.WalkSpeed;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        lastInput = Vector2.zero;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        lastInput = ctx.ReadValue<Vector2>();
        playerBrain.LastPlayerMovement = lastInput;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "Inputs : " + lastInput.ToString());
    }
}
