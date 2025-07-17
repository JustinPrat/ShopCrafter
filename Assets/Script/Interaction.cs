using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    private InputReceiver inputReceiver;

    [SerializeField]
    private ControllerData controllerData;

    [SerializeField]
    private PlayerBrain playerBrain;

    [SerializeField]
    private SpriteRenderer iconHolder;

    private IInteractable lastInteractable;

    private void Start()
    {
        inputReceiver.Actions.Player.Interact.started += OnInteractStarted;
    }

    private void OnDestroy()
    {
        inputReceiver.Actions.Player.Interact.started -= OnInteractStarted;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, playerBrain.LastPlayerMovement, Color.yellow, 1f);
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, playerBrain.LastPlayerMovement, controllerData.InteractionRange, controllerData.InteractionLayer);
        IInteractable interacted = null;
        foreach (RaycastHit2D item in hit)
        {
            if (item && item.transform.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.CanInteract(playerBrain))
                {
                    iconHolder.gameObject.SetActive(true);
                    iconHolder.sprite = interactable.InteractIcon;
                    lastInteractable = interactable;
                }

                interacted = interactable;
                break;
            }
        }

        if (lastInteractable != null && interacted != lastInteractable)
        {
            lastInteractable.OutOfInteractRange(playerBrain);
            lastInteractable = null;
        }

        if (interacted == null)
        {
            if (iconHolder.gameObject.activeInHierarchy)
            {
                iconHolder.gameObject.SetActive(false);
            }
        }
    }

    private void OnInteractStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Interact");
        Debug.DrawRay(transform.position, playerBrain.LastPlayerMovement, Color.yellow, 1f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerBrain.LastPlayerMovement, controllerData.InteractionRange, controllerData.InteractionLayer);
        if (hit && hit.transform.TryGetComponent(out IInteractable interactable) && interactable.CanInteract(playerBrain))
        {
            Debug.Log("hit element : " + hit.transform.name);
            interactable.DoInteract(playerBrain);
        }
    }
}
