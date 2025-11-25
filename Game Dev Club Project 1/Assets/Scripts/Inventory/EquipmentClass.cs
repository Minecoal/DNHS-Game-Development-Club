using UnityEngine;

[CreateAssetMenu(fileName = "new Equipment Class", menuName = "Item/Equipment")]
public class EquipmentClass : ItemClass
{
    public void OnEquip(PlayerManager stats) { Debug.Log(itemName); }
    public void OnUnequip(PlayerManager stats) { }

    public override EquipmentClass GetEquipment() { return this; }
}
