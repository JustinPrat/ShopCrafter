using System.Collections.Generic;
using UnityEngine;

public class GameMetaDataManager : MonoBehaviour, IInitializableManager
{
    [SerializeField]
    private ManagerRefs managerRefs;

    private int moneyToday;
    private int moneyTotal;
    private bool playerInsideTrain;

    private List<CraftedObjectData> craftedObjectsToday = new List<CraftedObjectData>();
    private List<CraftedObjectData> craftedObjectsTotal = new List<CraftedObjectData>();

    public int MoneyToday => moneyToday;
    public int MoneyTotal => moneyTotal;

    public IReadOnlyList<CraftedObjectData> CraftedRecipesToday => craftedObjectsToday;
    public IReadOnlyList<CraftedObjectData> CraftedRecipesTotal => craftedObjectsTotal;

    public bool PlayerInsideTrain => playerInsideTrain;

    public void Register(ManagerRefs refs)
    {
        managerRefs.GameMetaDataManager = this;
    }

    public void Init(ManagerRefs refs)
    {
        managerRefs.GameEventsManager.OnMoneyGained += OnMoneyGained;
        managerRefs.GameEventsManager.dayEvents.OnStartDay += OnStartDay;
        managerRefs.GameEventsManager.craftEvents.OnCraftedItem += OnCraftedItem;
        managerRefs.GameEventsManager.playerEvents.OnPlayerZoneTrain += OnPlayerZoneTrain;
    }

    private void OnPlayerZoneTrain(bool isInside)
    {
        playerInsideTrain = isInside;
    }

    private void OnDestroy()
    {
        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.OnMoneyGained -= OnMoneyGained;
            managerRefs.GameEventsManager.dayEvents.OnStartDay -= OnStartDay;
            managerRefs.GameEventsManager.craftEvents.OnCraftedItem -= OnCraftedItem;
            managerRefs.GameEventsManager.playerEvents.OnPlayerZoneTrain -= OnPlayerZoneTrain;
        }
    }

    private void OnCraftedItem(CraftedObjectData craftedObject)
    {
        craftedObjectsToday.Add(craftedObject);
        craftedObjectsTotal.Add(craftedObject);
    }

    private void OnStartDay()
    {
        moneyToday = 0;
        craftedObjectsToday.Clear();
    }

    private void OnMoneyGained(int money)
    {
        moneyToday += money;
        moneyTotal += money;
    }
}
