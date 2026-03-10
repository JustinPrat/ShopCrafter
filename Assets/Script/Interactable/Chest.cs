using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Collider2D collider;

    [SerializeField] 
    private Sprite icon;
    public Sprite InteractIcon => icon;

    public bool IsLocked { get; set; }
    public Collider2D Collider => collider;
    public GameObject GameObject => gameObject;

    private List<CraftedObjectData> craftedInventory = new List<CraftedObjectData>();

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        //Open chest UI
    }

    public void OnInteractRange(PlayerBrain playerBrain)
    {
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
    }
}
