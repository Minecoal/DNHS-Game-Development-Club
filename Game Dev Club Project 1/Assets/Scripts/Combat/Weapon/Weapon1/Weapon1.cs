using UnityEngine;
using System.Collections;

public class Weapon1 : IWeapon
{
    [SerializeField] private AttackData slashAttack;
    [SerializeField] private AttackData slashAttackFT;
    [SerializeField] private AttackData heavyAttack;
    [SerializeField] private AttackData dashAttack;

    private AttackData[] combo1;
    private float comboResetTime = 1f;
    private Coroutine resetCoroutine;

    void Awake()
    {
        canAttack = true;
        combo1 = new AttackData[]{
            slashAttack,
            slashAttackFT,
            slashAttack,
            heavyAttack,
        };
    }

    private int comboIndex = 0;

    override public bool TryAttack(PlayerContext context, bool isDashing)
    {
        if (!canAttack)
            return false;

        AttackData attackData;
        if (isDashing)
        {
            attackData = dashAttack;
        }
        else
        {
            attackData = combo1[comboIndex % combo1.Length];
            comboIndex++;
        }

        AttackCommand attack = new AttackCommand(context, attackData);
        attack.Execute();
        StartCoroutine(AttackCooldownCoroutine(attackData));

        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ComboResetCoroutine());

        return true;
    }

    private IEnumerator ComboResetCoroutine()
    {
        // wait for inactivity
        yield return new WaitForSeconds(comboResetTime);

        comboIndex = 0;
        resetCoroutine = null;
    }
}