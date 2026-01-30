using TMPEffects.TMPEvents;

public interface IPNJTraitRuntime
{
    void OnSpawn(PNJBrain pnjBrain);

    void OnUpdate(PNJBrain pnjBrain);

    void OnDespawn(PNJBrain pnjBrain);

    public virtual void OnInteract(PNJBrain pnjBrain) { }

    public virtual void OnTextEvent(TMPEventArgs args) { }

    public virtual void OnItemBuy(SellingItem clickedItem, ItemShopUI itemShopUI) { }
}
