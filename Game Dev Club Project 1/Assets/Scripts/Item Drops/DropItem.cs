using UnityEngine;

public class DropItem : MonoBehaviour
{
    private int maxItemDropStack = 10;

    private GameObject droppedItemPrefab;

    private void Start()
    {
        droppedItemPrefab = InventoryManager.Instance.droppedItemPrefab;
    }

    public void DropItems(ItemSlot itemToDrop)
    {
        GameObject droppedItem = Instantiate(droppedItemPrefab, PlayerManager.Instance.Player.transform.position, Quaternion.identity);
        droppedItem.GetComponent<DroppedItem>().SetDroppedItem(itemToDrop);
        ShootOut(droppedItem, Random.Range(1f, 3f));
    }

    public void DropItems(ItemSlot itemToDrop, Vector3 dropPosition)
    {
        GameObject droppedItem = Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);
        droppedItem.GetComponent<DroppedItem>().SetDroppedItem(itemToDrop);
        ShootOut(droppedItem, Random.Range(1f, 3f));   
    }

    public void DropItems(DropTableClass[] itemsToDrop, Vector3 dropPosition)
    {
        foreach(DropTableClass d in itemsToDrop)
        {
            int roll = Random.Range(1, 101);
            if (roll <= d.dropChance)
            {
                int numToDrop = Random.Range(d.minQuantity, d.maxQuantity + 1);
                //if item drop more than maxItemDropStack, condense it into 1 gameobject
                int numOfGameObjects = Mathf.FloorToInt(numToDrop / maxItemDropStack);
                if (numOfGameObjects == 0)
                {
                    for (int i = 0; i < numToDrop; i++)
                    {
                        GameObject droppedItem = Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);
                        droppedItem.GetComponent<DroppedItem>().SetDroppedItem(new ItemSlot(d.itemDropped, 1));
                        ShootOut(droppedItem, Random.Range(1f, 3f));
                    }
                }
                else
                {
                    int numLeft = numToDrop;
                    for (int i = 0; i <= numOfGameObjects; i++)
                    {
                        int n = numLeft >= maxItemDropStack ? maxItemDropStack : numLeft % maxItemDropStack;
                        GameObject droppedItem = Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);
                        droppedItem.GetComponent<DroppedItem>().SetDroppedItem(new ItemSlot(d.itemDropped, n));
                        numLeft -= n;
                        ShootOut(droppedItem, Random.Range(1f, 3f));
                    }
                }
            }
        }
    }

    public void DropCurrency(int money, Vector3 dropPosition)
    {
        GameObject droppedItem = Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);
        droppedItem.GetComponent<DroppedItem>().SetCurrency(money, null);
        ShootOut(droppedItem, Random.Range(1f, 3f));
    }

    public static void ShootOut(GameObject item, float force)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 dir = Random.insideUnitSphere;
        dir.y = 0.3f;
        dir.Normalize();

        rb.AddForce(dir * force, ForceMode.Impulse);
    }
}
