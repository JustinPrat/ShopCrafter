using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement3D : MonoBehaviour
{
    [SerializeField] 
    private ControllerData controllerData;

    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private PlayerBrain playerBrain;

    private Vector2 lastInput;
    private Rigidbody rigidBody;

    private void Start()
    {
        managerRefs.InputManager.Actions.Player.Move.performed += OnMovePerformed;
        managerRefs.InputManager.Actions.Player.Move.canceled += OnMoveCanceled;
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.Player.Move.performed -= OnMovePerformed;
        managerRefs.InputManager.Actions.Player.Move.canceled -= OnMoveCanceled;
    }

    private void Update()
    {
        Vector2 move = lastInput * controllerData.WalkSpeed;
        rigidBody.linearVelocity = new Vector3(move.x, 0, move.y);
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        lastInput = Vector2.zero;
        playerBrain.StopMovementPlayer();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        lastInput = ctx.ReadValue<Vector2>();
        playerBrain.SetLastPlayerMovement(lastInput);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "Inputs : " + lastInput.ToString());
    }
}
