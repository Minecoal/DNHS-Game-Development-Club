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
       
        AttackCommand attack = new AttackCommand(context, slash);
        attack.Execute();
        StartCoroutine(AttackCooldownCoroutine(slash));
        return true;
    }
}
