using UnityEngine;

[CreateAssetMenu(fileName = "new Equipment Class", menuName = "Item/Equipment")]
public class EquipmentClass : ItemClass
{
    public virtual void OnEquip(PlayerContext context) { Debug.Log("equipped: " + itemName); }
    //effects
    public virtual void EquipmentEffect() { }
    public virtual void OnUnequip(PlayerContext context) { Debug.Log("unequipped: " + itemName); }

    public override EquipmentClass GetEquipment() { return this; }
}
