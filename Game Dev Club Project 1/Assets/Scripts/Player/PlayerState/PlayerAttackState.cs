using System;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerStateManager ctx;
    private PlayerInputHandler input;
    private PlayerData data;
    private Animator animator;
    private Rigidbody rb;

    private AttackData[] attackDataList;
    private Transform attackAnchor;
    private IAttack[] attackList;
    private int attackIndex = 0;

    public event Action<int> OnAttackStarted;
    public event Action<int, DamageResult, DamageInfo> OnAttackHit;
    public event Action<int> OnAttackReady;
    private bool isAttacking = true;

    private int bufferedInput = -1; // the index of the buffered input
    private float bufferTime;
    private float bufferTimestamp;

    public PlayerAttackState(AttackData[] attackDataList, Transform attackAnchor)
    {
        this.attackDataList = attackDataList;
        this.attackAnchor = attackAnchor;

        attackList = new IAttack[attackDataList.Length];
        for (int i = 0; i < attackDataList.Length; i++)
        {
            if (attackDataList[i].isProjectile)
            {
                // implement projectile attack when needed
                // attackList[i] = new ProjectileAttack(attackDataList[i], i, HandleAttackHit, HandleAttackComplete); 
            }
            else
            {
                attackList[i] = new MeleeAttack(attackDataList[i], i, HandleAttackHit, HandleAttackReady);
            }
        }
    }
    

    public void Enter(PlayerStateManager ctx)
    {
        this.ctx = ctx;
        input = this.ctx.Input;
        animator = this.ctx.Animator;
        rb = this.ctx.Rb;
        data = this.ctx.Data;

        animator.SetTrigger("EnterAttack");

        TryStartAttack(attackIndex);
        input.AttackPressed += OnAttack;
    }

    public void Exit(PlayerStateManager ctx)
    {
        input.AttackPressed -= OnAttack;
    }

    public void UpdateFixed(PlayerStateManager ctx)
    {

    }

    public void Update(PlayerStateManager ctx)
    {
        Decelerate();
        TestBufferedInput();

        if (isAttacking) return;
        if (input.MoveInput.sqrMagnitude > 0.01f)
        {
            ctx.SwitchState(ctx.MovingState);
        }
        else
        {
            ctx.SwitchState(ctx.IdleState);
        }
    }

    private void StartAttack(int index)
    {
        AttackData data = attackDataList[index];
        OnAttackStarted?.Invoke(index);
        isAttacking = true;
        ctx.StartCoroutine(attackList[index].ReadyAfterCooldown());

        if (!string.IsNullOrEmpty(data.animatorTrigger))
        {
            animator.SetTrigger(data.animatorTrigger);
        }

        // if using animation events to time the hit, don't execute here, otherwise call Execute immediately
        if (!data.useAnimationEvent)
        {
            // spawn hit effects at attackAnchor, but pass the player transform as the logical originator
            attackList[index].ExecuteAttack(attackAnchor, ctx.transform, input.MoveInput);
        }
    }

    public void OnAnimationAttack(int attackIndex)
    {
        attackList[attackIndex].ExecuteAttack(attackAnchor, ctx.transform, input.MoveInput);
    }

    public void TryStartAttack(int index)
    {
        if (attackList == null || index < 0 || index >= attackList.Length) return;

        if (attackList[index].IsAvailable())
        {
            StartAttack(index);
        }
        else // buffer it
        {
            bufferedInput = index;
            bufferTimestamp = Time.time;
        }
    }

    private void TestBufferedInput()
    {
        if (bufferedInput >= 0 && Time.time - bufferTimestamp <= bufferTime)
        {
            if (attackList[bufferedInput].IsAvailable())
            {
                StartAttack(bufferedInput);
            }
        } else {
            bufferedInput = -1;
        }
    }

    private void OnAttack()
    {
        TryStartAttack(attackIndex);
    }

    private void HandleAttackHit(int attackIndex, DamageResult result, DamageInfo info)
    {
        // Debug.Log("Attack Landed");
        OnAttackHit?.Invoke(attackIndex, result, info);
    }

    private void HandleAttackReady(int attackIndex)
    {
        // Debug.Log("Attack Ready");
        OnAttackReady?.Invoke(attackIndex);
        isAttacking = false;
    }

    public void Decelerate()
    {
    Vector3 movement = new Vector3(-rb.linearVelocity.x * data.decelAmount, -rb.linearVelocity.y * data.decelAmount, 0f);
        rb.AddForce(movement.x * Vector3.right, ForceMode.Force);
        rb.AddForce(movement.y * Vector3.up, ForceMode.Force);
    }

    public override string ToString()
    {
        return "Attack";
    }
}