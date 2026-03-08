using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header(header: "Movement")]
    public float moveSpeed = 8;
    public float accelAmount = 12;
    public float decelAmount = 9;


    [Header(header: "Dash")]
    public float dashForce = 6;
    public float dashDuration = 0.2f;
    public float dashStaminaCost = 30;

    [Header(header: "Attack")]
    public float attackBonus = 0;

    [Header(header: "Stamina")]
    public float maxStamina = 100;
    public float staminaRechargeRate = 10; // stamina per second
    public float staminaRechargeDelay = 1.5f; // minimum delay before recharging


    public PlayerData Clone()
    {
        return new PlayerData
        {
            moveSpeed = this.moveSpeed,
            accelAmount = this.accelAmount,
            decelAmount = this.decelAmount,
            dashForce = this.dashForce,
            dashDuration = this.dashDuration,
            dashStaminaCost = this.dashStaminaCost,
            attackBonus = this.attackBonus,
            maxStamina = this.maxStamina,
            staminaRechargeRate = this.staminaRechargeRate,
            staminaRechargeDelay = this.staminaRechargeDelay

        };
    }
}
