using System;
using UnityEngine;

public class AttackLogic : MonoBehaviour
{
    public AttackData[] attackDataList; // assign in inspector
    private IAttack[] attackList;
    public Animator animator;
    [SerializeField] private Transform attackAnchor;

    // Attack events
    public event Action<int> OnAttackStarted;
    public event Action<int, DamageResult, DamageInfo> OnAttackHit;
    public event Action<int> OnAttackComplete;

    private int bufferedInput = -1; // the index of the buffered input
    private float bufferTime = 0.2f;
    private float bufferTimestamp;

    void Awake()
    {
        if (attackDataList == null) return;
        attackList = new IAttack[attackDataList.Length];
        for (int i = 0; i < attackDataList.Length; i++)
        {
            if (attackDataList[i].isProjectile)
            {
                // implement projectile attack when available
                attackList[i] = new MeleeAttack(attackDataList[i], i, HandleAttackHit);
            }
            else
            {
                attackList[i] = new MeleeAttack(attackDataList[i], i, HandleAttackHit);
            }
        }
    }

    void Update()
    {

        // test buffered input
        if (bufferedInput >= 0 && Time.time - bufferTimestamp <= bufferTime)
        {
            if (attackList[bufferedInput].IsAvailable())
            {
                StartAttack(bufferedInput);
            }
        } else
        {
            bufferedInput = -1;
        }
    }

    public void TryStartAttack(int index)
    {
        if (attackList == null || index < 0 || index >= attackList.Length) return;

        if (attackList[index].IsAvailable()) 
        {
            StartAttack(index);
        }
        else
        {
            // buffer it
            bufferedInput = index;
            bufferTimestamp = Time.time;
        }
    }

    private void StartAttack(int index)
    {
        AttackData data = attackDataList[index];

        OnAttackStarted?.Invoke(index);

        if (animator != null && !string.IsNullOrEmpty(data.animatorTrigger))
        {
            animator.SetTrigger(data.animatorTrigger);
        }

        // if using animation events to time the hit, don't execute here, otherwise call Execute immediately
        if (!data.useAnimationEvent)
        {
            // spawn hit effects at attackAnchor, but pass the player transform as the logical originator
            Transform spawnParent = attackAnchor != null ? attackAnchor : transform;
            attackList[index].ExecuteAttack(spawnParent, transform, PlayerManager.Instance.moveInput);
        }
    }

    // called from animation event
    public void OnAnimationAttack(int attackIndex)
    {
        attackList[attackIndex].ExecuteAttack(attackAnchor, transform, PlayerManager.Instance.moveInput);
    }

    // internal handler forwarded into MeleeAttack
    private void HandleAttackHit(int attackIndex, DamageResult result, DamageInfo info)
    {
        OnAttackHit?.Invoke(attackIndex, result, info);
    }

    private void HandleAttackComplete(int attackIndex)
    {
        OnAttackComplete?.Invoke(attackIndex);
    }
}