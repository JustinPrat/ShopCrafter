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

    [SerializeField]
    private Collider physicCollider;

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
        Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 5f, Color.red, Time.deltaTime);

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f, controllerData.GroundLayer))
        {
            rigidBody.position = new Vector3(rigidBody.position.x, hit.point.y + physicCollider.bounds.size.y / 2, rigidBody.position.z);

            Vector3 movement = Quaternion.FromToRotation(Vector3.up, hit.normal) * new Vector3(move.x, 0, move.y);
            rigidBody.linearVelocity = movement;

            DebugMovement(movement);
        }
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

    private void DebugMovement(Vector3 movement)
    {
#if UNITY_EDITOR
        if (controllerData.ToggleDebug)
        {
            Debug.DrawRay(transform.position - Vector3.down * 0.1f, movement.normalized, Color.blue, Time.deltaTime);
        }
#endif
    }

    private void OnGUI()
    {
        if (controllerData.ToggleDebug)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "Inputs : " + lastInput.ToString());
        }
    }
}
