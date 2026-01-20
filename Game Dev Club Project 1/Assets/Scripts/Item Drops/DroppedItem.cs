using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private ItemSlot item;
    private int currencyAmount;

    private InventoryManager inventory;
    private bool isItem = false;

    private float pickupDelay = 0.5f;
    private float spawnTime;

    private void Awake()
    {
        spawnTime = Time.time;
    }

    private void Start()
    {
        inventory = InventoryManager.Instance;
    }
    
    public void SetDroppedItem(ItemSlot itemSlot)
    {
        this.item = itemSlot;
        isItem = true;
        GetComponent<SpriteRenderer>().sprite = item.GetItem().itemIcon;
    }

    public void SetCurrency(int amount, Sprite coinSprite)
    {
        currencyAmount = amount;
        //GetComponent<SpriteRenderer>().sprite = coinSprite;
        isItem = false;
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            if (isItem)
            {
                if (Time.time - spawnTime < pickupDelay)
                    return;

                int quantityToAdd = inventory.CanAddItem(item.GetItem(), item.GetQuantity());
                //can add all of item
                if (quantityToAdd == -1)
                {
                    inventory.AddItem(item.GetItem(), item.GetQuantity());
                    Destroy(gameObject);

                }
                //
                else if (quantityToAdd != 0)
                {
                    Debug.Log(quantityToAdd);
                    item.SubQuantity(quantityToAdd);
                    inventory.AddItem(item.GetItem(), quantityToAdd);
                    if (item.GetQuantity() == 0)
                    {
                        Destroy(this.gameObject);
                    }

                }
            }
            else
            {

                if (Time.time - spawnTime < pickupDelay)
                    return;

                inventory.AddCurrency(currencyAmount);
                Destroy(gameObject);
            }
        }
        
    }

   

}
