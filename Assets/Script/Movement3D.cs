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

    [SerializeField]
    private float radiusOffset = 0.1f;

    private float heightOffset;
    private float colliderRadius;
    private float currentGroundY;
    private Vector2 lastInput;
    private Rigidbody rigidBody;
    Vector3 rayOffset;

#if UNITY_EDITOR
    private Vector3 groundHitPos;
    private Vector3 nextHitPos;
#endif

    private void Start()
    {
        managerRefs.InputManager.Actions.Player.Move.performed += OnMovePerformed;
        managerRefs.InputManager.Actions.Player.Move.canceled += OnMoveCanceled;
        rigidBody = GetComponent<Rigidbody>();

        heightOffset = physicCollider.bounds.size.y / 2;
        colliderRadius = (physicCollider.bounds.size.x / 2) + radiusOffset;
        rayOffset = Vector3.up * (heightOffset + 0.5f);
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.Player.Move.performed -= OnMovePerformed;
        managerRefs.InputManager.Actions.Player.Move.canceled -= OnMoveCanceled;
    }

    private void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(lastInput.x, 0, lastInput.y);

        if (moveInput.sqrMagnitude < 0.01f)
        {
            rigidBody.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 flatMoveDir = moveInput.normalized;
        Vector3 targetVelocity = Vector3.zero;
        Vector3 moveDir = moveInput;

        AdaptDirectionToGroundNormal(ref moveDir, moveInput);
        TryProcessMovement(ref targetVelocity, moveDir, flatMoveDir, out bool isFullMovementValid);

        if (!isFullMovementValid)
        {
            ProcessSliding(ref targetVelocity, moveDir, flatMoveDir);
        }

        rigidBody.linearVelocity = targetVelocity;
    }

    private void AdaptDirectionToGroundNormal(ref Vector3 moveDir, Vector3 moveInput)
    {
        currentGroundY = rigidBody.position.y - heightOffset;
        if (Physics.Raycast(rigidBody.position + rayOffset, Vector3.down, out RaycastHit currentHit, 3f, controllerData.GroundLayer))
        {
#if UNITY_EDITOR
            groundHitPos = currentHit.point;
#endif
            moveDir = Vector3.ProjectOnPlane(moveInput, currentHit.normal).normalized;
            currentGroundY = currentHit.point.y;
        }
    }

    private void TryProcessMovement(ref Vector3 targetVelocity, Vector3 slopedDir, Vector3 flatDir, out bool isFullMovementValid)
    {
        Vector3 currentDirectionHitPoint = ProcessMovementDirection(flatDir, out isFullMovementValid);

        if (isFullMovementValid)
        {
#if UNITY_EDITOR
            nextHitPos = currentDirectionHitPoint;
#endif
            targetVelocity = slopedDir * controllerData.WalkSpeed;
            targetVelocity.y = (currentDirectionHitPoint.y + heightOffset - rigidBody.position.y) / Time.fixedDeltaTime;
        }
    }

    private void ProcessSliding(ref Vector3 targetVelocity, Vector3 slopedDir, Vector3 flatDir)
    {
        Vector3[] flatSlideDirs = { new Vector3(flatDir.x, 0, 0), new Vector3(0, 0, flatDir.z) };
        Vector3[] slopedSlideDirs = { new Vector3(slopedDir.x, 0, 0), new Vector3(0, 0, slopedDir.z) };

        bool setY = false;
        Vector3 maxHitPoint = rigidBody.position - (Vector3.up * 10f);
        Vector3 finalSlideVelocity = Vector3.zero;

        for (int i = 0; i < 2; i++)
        {
            if (flatSlideDirs[i].sqrMagnitude < 0.001f) continue;

            Vector3 currentDirectionHitPoint = ProcessMovementDirection(flatSlideDirs[i], out bool validDirection);

            if (validDirection)
            {
                finalSlideVelocity += slopedSlideDirs[i] * controllerData.WalkSpeed;

                if (!setY || currentDirectionHitPoint.y > maxHitPoint.y)
                {
                    setY = true;
                    maxHitPoint = currentDirectionHitPoint;
                }
            }
        }

        if (setY)
        {
#if UNITY_EDITOR
            nextHitPos = maxHitPoint;
#endif
            targetVelocity = finalSlideVelocity;
            targetVelocity.y = (maxHitPoint.y + heightOffset - rigidBody.position.y) / Time.fixedDeltaTime;
        }
    }

    private Vector3 ProcessMovementDirection(Vector3 flatDirection, out bool validDirection)
    {
        validDirection = false;
        Vector3 currentDirectionHitPoint = Vector3.zero;

        if (flatDirection.sqrMagnitude < 0.001f)
            return currentDirectionHitPoint;

        Vector3 checkDirection = flatDirection.normalized;
        float frameDistance = controllerData.WalkSpeed * flatDirection.magnitude * Time.fixedDeltaTime;

        Vector3 rayStartForward = rigidBody.position + (checkDirection * colliderRadius) + rayOffset;

        if (Physics.Raycast(rayStartForward, Vector3.down, out RaycastHit hit, 4f, controllerData.GroundLayer))
        {
            float heightDifference = hit.point.y - currentGroundY;

            if (heightDifference <= controllerData.MaxStepHeight && heightDifference > controllerData.MaxDownHeight)
            {
                validDirection = true;
                currentDirectionHitPoint = hit.point;

                if (heightDifference <= 0.02f)
                {
                    Vector3 rayStartExact = rigidBody.position + (checkDirection * frameDistance) + rayOffset;
                    if (Physics.Raycast(rayStartExact, Vector3.down, out RaycastHit exactHit, 4f, controllerData.GroundLayer))
                    {
                        currentDirectionHitPoint = exactHit.point;
                    }
                }
            }
        }

        return currentDirectionHitPoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(groundHitPos, 0.05f);

        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(nextHitPos, 0.05f);
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
