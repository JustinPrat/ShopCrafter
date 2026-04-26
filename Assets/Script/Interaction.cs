using System.Collections.Generic;
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

    private IInteractable currentInteractable;
    private List<IInteractable> interactablesInRange = new List<IInteractable>();

    private void Start()
    {
        managerRefs.InputManager.Actions.Player.Interact.started += OnInteractStarted;
    }

    private void OnDestroy()
    {
        managerRefs.InputManager.Actions.Player.Interact.started -= OnInteractStarted;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.TryGetComponent(out IInteractable interactable) && !interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.TryGetComponent(out IInteractable interactable) && interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Remove(interactable);
        }
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, new Vector3(playerBrain.LastPlayerMovement.x, 0, playerBrain.LastPlayerMovement.y), Color.yellow, Time.deltaTime);

        IInteractable newInteractable = null;
        float highestDot = -1f;

        foreach (IInteractable interactable in interactablesInRange)
        {
            Vector2 interactDir = new Vector2(interactable.GameObject.transform.position.x - transform.position.x, interactable.GameObject.transform.position.z - transform.position.z).normalized;
            float dot = Vector2.Dot(playerBrain.LastPlayerMovement.normalized, interactDir);
            if (interactable.CanInteract(playerBrain) && !interactable.IsLocked && dot > highestDot)
            {
                newInteractable = interactable;
                highestDot = dot;
            }
        }

        if (currentInteractable != null && newInteractable != currentInteractable)
        {
            currentInteractable.OutOfInteractRange(playerBrain);
            currentInteractable = null;
        }

        if (currentInteractable == null && newInteractable != null)
        {
            newInteractable.OnInteractRange(playerBrain);

            iconHolder.gameObject.SetActive(true);
            iconHolder.sprite = newInteractable.InteractIcon;
        }

        if (newInteractable == null || !newInteractable.CanInteract(playerBrain) || newInteractable.IsLocked || !managerRefs.InputManager.Actions.Player.Interact.enabled)
        {
            if (iconHolder.gameObject.activeInHierarchy)
            {
                iconHolder.gameObject.SetActive(false);
            }
        }

        currentInteractable = newInteractable;
    }

    private void OnInteractStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Interact");
        Debug.DrawRay(transform.position, playerBrain.LastPlayerMovement, Color.blue, 1f);

        currentInteractable?.DoInteract(playerBrain);
    }
}
