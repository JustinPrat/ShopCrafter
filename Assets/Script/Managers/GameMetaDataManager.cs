using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMetaDataManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    private int moneyToday;
    private int moneyTotal;

    private List<CraftedObjectData> craftedObjectsToday = new List<CraftedObjectData>();
    private List<CraftedObjectData> craftedObjectsTotal = new List<CraftedObjectData>();

    public int MoneyToday => moneyToday;
    public int MoneyTotal => moneyTotal;

    public IReadOnlyList<CraftedObjectData> CraftedRecipesToday => craftedObjectsToday;
    public IReadOnlyList<CraftedObjectData> CraftedRecipesTotal => craftedObjectsTotal;


    private void Awake()
    {
        managerRefs.GameMetaDataManager = this;
    }

    private void Start()
    {
        //SUB to data changes
        managerRefs.GameEventsManager.OnMoneyGained += OnMoneyGained;
        managerRefs.GameEventsManager.dayEvents.OnStartDay += OnStartDay;
        managerRefs.GameEventsManager.craftEvents.OnCraftedItem += OnCraftedItem;
    }

    private void OnDestroy()
    {
        if (managerRefs.GameEventsManager)
        {
            managerRefs.GameEventsManager.OnMoneyGained -= OnMoneyGained;
            managerRefs.GameEventsManager.dayEvents.OnStartDay -= OnStartDay;
            managerRefs.GameEventsManager.craftEvents.OnCraftedItem -= OnCraftedItem;
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
