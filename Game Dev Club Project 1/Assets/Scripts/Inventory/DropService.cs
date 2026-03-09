using UnityEngine;

public class DropService
{
    private int currency = 0;
    private DropItem dropItemComponent;

    public DropService(DropItem dropItemComponent)
    {
        this.dropItemComponent = dropItemComponent;
    }

    public void AddCurrency(int amount)
    {
        if (amount <= 0) return;

        currency += amount;
        InventoryEvents.PublishCurrencyAdded(amount);
    }

    public void RemoveCurrency(int amount)
    {
        if (amount <= 0) return;

        currency = Mathf.Max(0, currency - amount);
        InventoryEvents.PublishCurrencyRemoved(amount);
    }

    public int GetCurrency()
    {
        return currency;
    }

    public void DropItems(ItemSlot itemToDrop, Vector3 position)
    {
        if (itemToDrop.GetItem() == null) return;

        dropItemComponent.DropItems(itemToDrop, position);
        InventoryEvents.PublishItemDropped(itemToDrop, position);
    }

    public void DropCurrency(int amount, Vector3 position)
    {
        if (amount <= 0) return;

        dropItemComponent.DropCurrency(amount, position);
        InventoryEvents.PublishCurrencyDropped(amount, position);
    }
}
