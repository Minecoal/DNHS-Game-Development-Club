using UnityEngine;

[CreateAssetMenu(fileName = "new Equipment Class", menuName = "Item/Equipment")]
public class EquipmentClass : ItemClass
{
    public void OnEquip() { Debug.Log("equipped: " + itemName); }
    //effects
    public void EquipmentEffect() { }
    public void OnUnequip() { Debug.Log("unequipped: " + itemName); }

    public override EquipmentClass GetEquipment() { return this; }
}
