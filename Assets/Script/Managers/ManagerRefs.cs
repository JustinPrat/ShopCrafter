using UnityEngine;

[CreateAssetMenu(fileName = "ManagerRefs", menuName = "ShopCrafter/ManagerRefs")]
public class ManagerRefs : ScriptableObject
{
    public SellManager SellManager { get; set; }
    public PNJManager PNJManager { get; set; }
    public CraftingManager CraftingManager { get; set; }
    public UIManager UIManager { get; set; }
    public DialogueManager DialogueManager { get; set; }
    public InputReceiver InputManager { get; set; }
    public GameEventsManager GameEventsManager { get; set; }
    public QuestManager QuestManager { get; set; }
}
