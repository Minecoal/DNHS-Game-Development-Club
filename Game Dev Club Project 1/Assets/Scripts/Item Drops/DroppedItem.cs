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

    private float flyTime = 0.3f;
    [SerializeField] private AnimationCurve flyCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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

                if (inventory.CanAddItem(item.GetItem(), item.GetQuantity()) != 0)
                    StartCoroutine(FlyToPlayer(col.transform));

                
            }
            else
            {

                if (Time.time - spawnTime < pickupDelay)
                    return;

                StartCoroutine(FlyToPlayer(col.transform));

                
                //Destroy(gameObject);
            }
        }
        
    }


    private IEnumerator FlyToPlayer(Transform target)
    {
        Vector3 startPos = transform.position;
        float t = 0f;


        while (t < flyTime)
        {
            t += Time.deltaTime;
            float normalized = t / flyTime;
            float curved = flyCurve.Evaluate(normalized);


            transform.position = Vector3.Lerp(startPos, target.position, curved);
            yield return null;
        }


        transform.position = target.position;
        

        if (isItem)
        {
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
            inventory.AddCurrency(currencyAmount);
            Destroy(gameObject);
        }

    }


}
