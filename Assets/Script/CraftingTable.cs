using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CraftingTable : WorldViewInteract<CraftingView>
{
    [SerializeField]
    private Transform UICraftAnchor;

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

    public override bool CanInteract(PlayerBrain playerBrain)
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

    public override void StartInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleCraftingView(true, this, UICraftAnchor == null ? transform.position : UICraftAnchor.position);

        if (interactSequencer != null)
            interactSequencer.StartSequence();

        base.StartInteract(playerBrain);
    }

    public Vector3 GetValidUIPos()
    {
        return UICraftAnchor == null ? transform.position : UICraftAnchor.position;
    }

    public void ExitInteract()
    {
        if (outInteractSequencer != null)
            outInteractSequencer.StartSequence();
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
}
