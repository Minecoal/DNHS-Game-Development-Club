using UnityEngine;

public class Weapon2 : IWeapon
{
    [SerializeField] private AttackData slash;

    void Awake()
    {
        canAttack = true;
    }

    override public bool TryAttack(PlayerContext context, bool isDashing)
    {
        if (!canAttack)
            return false;

        context.Stamina.UseStamina(slash.staminaCost);

        AttackCommand attack = new AttackCommand(context, slash);
        attack.Execute();
        StartCoroutine(AttackCooldownCoroutine(slash));
        return true;
    }

    public override bool CanAttack(PlayerContext context, bool isDashing)
    {
        if (!canAttack)
            return false;

        return context.Stamina.CanUseStamina(slash.staminaCost);
    }
}
