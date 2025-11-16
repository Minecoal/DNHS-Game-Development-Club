using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    [SerializeField] private ItemClass item;
    [SerializeField] private int quantity;

    public ItemSlot()
    {
        item = null;
        quantity = 0;
    }

    public ItemSlot(ItemClass _item, int _quantity)
    {
        item = _item;
        quantity = _quantity;
    }
    public ItemSlot(ItemSlot slot)
    {
        item = slot.item;
        quantity = slot.quantity;
    }

    public void Clear()
    {
        this.item = null;
        this.quantity = 0;
    }

    public ItemClass GetItem() { return item; }
    public int GetQuantity() { return quantity; }
    public void AddQuantity(int quantity) { this.quantity += quantity; }
    public void SubQuantity(int quantity) { this.quantity -= quantity; }
    public void AddItem(ItemClass item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
