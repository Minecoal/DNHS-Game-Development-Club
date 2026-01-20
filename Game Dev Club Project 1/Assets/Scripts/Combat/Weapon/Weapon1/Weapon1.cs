using System;
using System.Collections;
using UnityEngine;

public class Weapon1 : MonoBehaviour, IWeapon
{
    [SerializeField] private AttackData slash;
    [SerializeField] private AttackData heavy;

    public Action OnEnableSwitchState { get; set; }

    private bool canAttack = true;

    private int temp = 0;

    public void Onable()
    {
        canAttack = true;       
    }

    public bool TryAttack(PlayerContext context)
    {
        if (!canAttack)
            return false;
        // temperary attack logic : TODO change this logic
        if (temp % 2 == 0){ 
            AttackCommand attack = AttackCommand.Create<Attack_Melee_Slash>(context, slash);
            attack.Execute();
            StartCoroutine(AttackCooldownCoroutine(slash));
            temp++;
        } else {
            AttackCommand attack = AttackCommand.Create<Attack_Melee_Heavy>(context, heavy);
            attack.Execute();
            StartCoroutine(AttackCooldownCoroutine(heavy));
            temp++;
        }
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