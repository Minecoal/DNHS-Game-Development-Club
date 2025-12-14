using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private ItemSlot item;
    private bool canPickUp = false;

    public void SetDroppedItem(ItemSlot itemSlot)
    {
        this.item = itemSlot;

        GetComponent<SpriteRenderer>().sprite = item.GetItem().itemIcon;
        StartCoroutine(PickUpDelay());
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (canPickUp)
        {
            if (col.tag == "Player")
            {
                if (InventoryManager.Instance.CanAddItem(item.GetItem(), item.GetQuantity()) == -1)
                {
                    InventoryManager.Instance.AddItem(item.GetItem(), item.GetQuantity());
                    Destroy(gameObject);

                }
                else if (InventoryManager.Instance.CanAddItem(item.GetItem(), item.GetQuantity()) != 0)
                {
                    Debug.Log(InventoryManager.Instance.CanAddItem(item.GetItem(), item.GetQuantity()));
                    item.SubQuantity(InventoryManager.Instance.CanAddItem(item.GetItem(), item.GetQuantity()));
                    InventoryManager.Instance.AddItem(item.GetItem(), InventoryManager.Instance.CanAddItem(item.GetItem(), item.GetQuantity()));
                    if (item.GetQuantity() == 0)
                    {
                        Destroy(this.gameObject);
                    }

                }

            }
        }
    }

    IEnumerator PickUpDelay()
    {
        yield return new WaitForSeconds(2f);
        canPickUp = true;
    }

}
