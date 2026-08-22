using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private string interactText;

    [SerializeField]
    private Collider collider;

    [SerializeField]
    private Transform UICraftAnchor;

    public Collider PhysicCollider => collider;

    [SerializeField]
    private List<CraftedItemReceiver> receiveSlots;

    [SerializeField]
    private Sequencer.Sequencer sequencer;

    [SerializeField]
    private CinemachineCamera cinemachineFocusCamera;

    [SerializeField]
    private Sequencer.Sequencer interactSequencer;

    [SerializeField]
    private Sequencer.Sequencer outInteractSequencer;

    public List<CraftedItemReceiver> ReceiveSlots => receiveSlots;
    public GameObject GameObject => gameObject;
    public string InteractText => interactText;
    public bool IsLocked { get; set; }
    public Action<IInteractable> OnDestroyEvent { get; set; }

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public bool CanInteract(PlayerBrain playerBrain)
    {
        foreach (CraftedItemReceiver slots in receiveSlots)
        {
            if (!slots.HasHeldItem)
            {
                return true;
            }
        }
        return false;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleCraftingView(true, this, UICraftAnchor == null ? transform.position : UICraftAnchor.position);

        if (interactSequencer != null)
            interactSequencer.StartSequence();
    }

    public void ExitInteract()
    {
        if (outInteractSequencer != null)
            outInteractSequencer.StartSequence();
    }

    public void OnTargeted(PlayerBrain playerBrain)
    {
    }

    public void UnTargeted(PlayerBrain playerBrain)
    {
    }

    public void SpawnCraftedItem (CraftedObject craftedObject)
    {
        foreach (CraftedItemReceiver itemReceiver in receiveSlots)
        {
            if (!itemReceiver.HasHeldItem)
            {
                itemReceiver.SetItem(craftedObject);

                if (cinemachineFocusCamera != null)
                {
                    CameraTarget cameraTarget = new CameraTarget() { TrackingTarget = itemReceiver.transform };
                    cinemachineFocusCamera.Target = cameraTarget;
                }

                break;
            }
        }

        if (sequencer != null)
            sequencer.StartSequence();
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutInteractRange(PlayerBrain playerBrain)
    {
    }
}
