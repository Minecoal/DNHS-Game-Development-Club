using UnityEngine;

public class DropItem : MonoBehaviour
{
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
