using System;
using System.Collections;
using UnityEngine;

public class Weapon2 : MonoBehaviour, IWeapon
{
    [SerializeField] private AttackData slash;

    public Action OnEnableSwitchState { get; set; }

    private bool canAttack = true;

    public void Onable()
    {
        canAttack = true;
    }

    public bool TryAttack(PlayerContext context)
    {
        if (!canAttack)
            return false;
       
        AttackCommand attack = AttackCommand.Create<Attack_Melee_Slash>(context, slash);
        attack.Execute();
        StartCoroutine(AttackCooldownCoroutine(slash));
        return true;
    }

    public IEnumerator AttackCooldownCoroutine(AttackData data)
    {
        canAttack = false;
        yield return new WaitForSeconds(data.cooldown);
        canAttack = true;
        OnEnableSwitchState?.Invoke();
    }
}
