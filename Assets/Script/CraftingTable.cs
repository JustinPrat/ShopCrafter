using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField] 
    private Sprite icon;

    public Sprite InteractIcon => icon;

    public bool CanInteract(PlayerBrain playerBrain)
    {
        return true;
    }

    public void DoInteract(PlayerBrain playerBrain)
    {
        managerRefs.UIManager.ToggleCraftingView(true, transform.position);
    }

    public void OutOfInteractRange(PlayerBrain playerBrain)
    {
        
    }
}
