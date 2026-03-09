using UnityEngine;

public class InventoryStorageService
{
    private ItemSlot[] items;
    private DropItem dropItemComponent;
    private Transform playerTransform;

    public InventoryStorageService(int slotCount, DropItem dropItemComponent, Transform playerTransform)
    {
        items = new ItemSlot[slotCount];
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new ItemSlot();
        }
        this.dropItemComponent = dropItemComponent;
        this.playerTransform = playerTransform;
    }

    public void AddItem(ItemClass item, int quantity)
    {
        bool itemsAdded = false;
        int quantityLeft = quantity;

        ItemSlot slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
        {
            slot.AddQuantity(quantity);
            itemsAdded = true;
        }
        else
        {
            if (item.isStackable)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == null)
                    {
                        items[i].AddItem(item, quantity);
                        itemsAdded = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == null && quantityLeft > 0)
                    {
                        items[i].AddItem(item, 1);
                        quantityLeft--;
                        if (quantityLeft <= 0)
                        {
                            itemsAdded = true;
                            break;
                        }
                    }
                }
            }
        }

        if (!itemsAdded && quantityLeft > 0)
        {
            DropOverflow(item, quantityLeft);
        }

        InventoryEvents.PublishItemAdded(item, quantity);
        InventoryEvents.PublishInventoryRefreshed();
    }

    public void RemoveItem(ItemClass item)
    {
        ItemSlot temp = Contains(item);
        if (temp != null)
        {
            if (temp.GetQuantity() > 1)
                temp.SubQuantity(1);
            else
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == item)
                    {
                        items[i].Clear();
                        break;
                    }
                }
            }
        }
        InventoryEvents.PublishItemRemoved(item, 1);
        InventoryEvents.PublishInventoryRefreshed();
    }

    public void RemoveItem(ItemClass item, int quantity)
    {
        if (item.isStackable)
        {
            ItemSlot temp = Contains(item);
            if (temp != null)
            {
                if (temp.GetQuantity() > quantity)
                    temp.SubQuantity(quantity);
                else
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].GetItem() == item)
                        {
                            items[i].Clear();
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            int quantityLeft = quantity;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == item)
                {
                    items[i].Clear();
                    quantityLeft--;
                }
                if (quantityLeft <= 0)
                    break;
            }
        }
        InventoryEvents.PublishItemRemoved(item, quantity);
        InventoryEvents.PublishInventoryRefreshed();
    }

    public ItemSlot Contains(ItemClass item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item)
                return items[i];
        }
        return null;
    }

    public bool Contains(ItemClass item, int quantity)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item && items[i].GetQuantity() >= quantity)
                return true;
        }
        return false;
    }

    public int ContainAmount(ItemClass item)
    {
        if (item.isStackable)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == item)
                    return items[i].GetQuantity();
            }
        }
        else
        {
            int totalAmount = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == item)
                    totalAmount += items[i].GetQuantity();
            }
            return totalAmount;
        }
        return 0;
    }

    public int CanAddItem(ItemClass item, int quantity)
    {
        bool itemsAdded = false;
        int quantityLeft = quantity;
        int quantityAdded = 0;

        ItemSlot slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
            return -1;
        else
        {
            if (item.isStackable)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == null)
                    {
                        itemsAdded = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == null && quantityLeft > 0)
                    {
                        quantityLeft--;
                        quantityAdded++;
                    }
                    else if (quantityLeft == 0)
                    {
                        itemsAdded = true;
                        break;
                    }
                }
            }
        }

        if (!itemsAdded)
        {
            return quantityAdded;
        }

        return -1;
    }

    public ItemSlot[] GetItems()
    {
        return items;
    }

    public ItemSlot GetItemAt(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Length)
            return items[slotIndex];
        return null;
    }

    public void SetItemAt(int slotIndex, ItemSlot item)
    {
        if (slotIndex >= 0 && slotIndex < items.Length)
        {
            items[slotIndex] = item;
            InventoryEvents.PublishInventoryRefreshed();
        }
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Length)
        {
            items[slotIndex].Clear();
            InventoryEvents.PublishInventoryRefreshed();
        }
    }

    public int GetSlotCount()
    {
        return items.Length;
    }

    public void DropOverflow(ItemClass item, int quantity)
    {
        if (dropItemComponent != null && playerTransform != null)
        {
            dropItemComponent.DropItems(new ItemSlot(item, quantity), playerTransform.position);
            InventoryEvents.PublishItemDropped(new ItemSlot(item, quantity), playerTransform.position);
            Debug.Log("Dropped items: " + quantity + ", " + item.itemName);
        }
    }
}
