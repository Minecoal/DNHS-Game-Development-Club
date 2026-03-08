using UnityEngine;

[CreateAssetMenu(fileName = "new Max Stamina Buff Equipment Class", menuName = "Item/Equipment/MaxStaminaUp")]
public class MaxStaminaBuffEquipment : EquipmentClass
{
    public float maxStaminaIncrease;

    public override void OnEquip(PlayerContext context)
    {
        context.Data.maxStamina += maxStaminaIncrease;
        context.Stamina.UpdateUI();
    }
    //effect
    public override void OnUnequip(PlayerContext context)
    {
        context.Data.maxStamina -= maxStaminaIncrease;
        context.Stamina.UpdateUI();
    }

    public override EquipmentClass GetEquipment() { return this; }
}
