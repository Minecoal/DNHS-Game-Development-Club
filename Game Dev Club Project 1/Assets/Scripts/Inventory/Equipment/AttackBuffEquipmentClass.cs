using UnityEngine;

[CreateAssetMenu(fileName = "new Attack Buff Equipment Class", menuName = "Item/Equipment/AttackUp")]
public class AttackBuffEquipmentClass: EquipmentClass
{
    public int attackBuff;

    public override void OnEquip(PlayerContext context)
    {
        context.Data.attackBonus += attackBuff;
    }
    //effect
    public override void OnUnequip(PlayerContext context)
    {
        context.Data.attackBonus -= attackBuff;
    }

    public override EquipmentClass GetEquipment() { return this; }
}
