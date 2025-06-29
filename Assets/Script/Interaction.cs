using UnityEngine;
using UnityEngine.EventSystems;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    private InputReceiver inputReceiver;

    [SerializeField]
    private ControllerData controllerData;

    [SerializeField]
    private PlayerBrain playerBrain;

    private void Start()
    {
        inputReceiver.Actions.Player.Interact.started += OnInteractStarted;
    }

    private void OnDestroy()
    {
        inputReceiver.Actions.Player.Interact.started -= OnInteractStarted;
    }

    private void OnInteractStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Interact");
        Debug.DrawRay(transform.position, playerBrain.LastPlayerMovement, Color.yellow, 1f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerBrain.LastPlayerMovement, controllerData.InteractionRange, controllerData.InteractionLayer);
        if (hit && hit.transform.TryGetComponent(out IInteractable interactable))
        {
            Debug.Log("hit element : " + hit.transform.name);
            interactable.DoInteract();
        }
    }
}
