using TMPEffects.TMPEvents;

public interface IPNJTraitRuntime
{
    void OnSpawn(PNJBrain pnjBrain);

    void OnUpdate(PNJBrain pnjBrain);

    void OnDespawn(PNJBrain pnjBrain);

    public void OnInteract(PNJBrain pnjBrain);

    public void OnTextEvent(TMPEventArgs args);

    public void OnItemBuy(SellingItem clickedItem, ItemShopUI itemShopUI);
}
