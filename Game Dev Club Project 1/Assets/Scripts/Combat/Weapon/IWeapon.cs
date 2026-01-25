using System;
using System.Collections;
using UnityEngine;


public abstract class IWeapon : MonoBehaviour
{
    protected bool canAttack;
    virtual public Action OnEnableSwitchState { get; set; }
    
    abstract public bool TryAttack(PlayerContext context, bool isDashing);
    
    virtual protected IEnumerator AttackCooldownCoroutine(AttackData data)
    {
        canAttack = false;
        yield return new WaitForSeconds(data.cooldown);
        canAttack = true;
        OnEnableSwitchState?.Invoke();
    }
} 