using UnityEngine;

[CreateAssetMenu(fileName = "new Tool Class", menuName = "Item/Consumable")]
public class ConsumableClass : ItemClass
{
    [Header("Consumable")]
    public ConsumableType consumableType;
    public enum ConsumableType
    {
        health
    }
    public float buffAmount;
    public override ConsumableClass GetConsumable() { return this; }
}
