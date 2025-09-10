using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private ControllerData controllerData;

    [SerializeField]
    private PlayerBrain playerBrain;

    [SerializeField]
    private SpriteRenderer iconHolder;

    private IInteractable lastInteractable;

    private void Start()
    {
        managerRefs.InputManager.Actions.Player.Interact.started += OnInteractStarted;
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.Player.Interact.started -= OnInteractStarted;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, playerBrain.LastPlayerMovement, Color.yellow, 1f);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, playerBrain.LastPlayerMovement, controllerData.InteractionRange, controllerData.InteractionLayer);
        IInteractable interactableInSight = null;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.transform.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.CanInteract(playerBrain))
                {
                    iconHolder.gameObject.SetActive(true);
                    iconHolder.sprite = interactable.InteractIcon;
                }
                
                interactableInSight = interactable;
                break;
            }
        }

        if (lastInteractable != null && interactableInSight != lastInteractable)
        {
            lastInteractable.OutOfInteractRange(playerBrain);
            lastInteractable = null;
        }

        if (lastInteractable == null && interactableInSight != null)
        {
            interactableInSight.OnInteractRange(playerBrain);
        }

        if (interactableInSight == null)
        {
            if (iconHolder.gameObject.activeInHierarchy)
            {
                iconHolder.gameObject.SetActive(false);
            }
        }

        lastInteractable = interactableInSight;
    }

    private void OnInteractStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Interact");
        Debug.DrawRay(transform.position, playerBrain.LastPlayerMovement, Color.yellow, 1f);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, playerBrain.LastPlayerMovement, controllerData.InteractionRange, controllerData.InteractionLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit && hit.transform.TryGetComponent(out IInteractable interactable) && interactable.CanInteract(playerBrain))
            {
                Debug.Log("hit element : " + hit.transform.name);
                interactable.DoInteract(playerBrain);
            }
        }
    }
}
