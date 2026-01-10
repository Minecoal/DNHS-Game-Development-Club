using System;
using UnityEngine;

public class PlayerDashingState : IPlayerState
{
    private PlayerStateMachine ctx;
    private PlayerInputHandler input;
    private PlayerData data;
    private Animator animator;
    private Rigidbody rb;

    private float dashStartTime;
    private Vector3 dashDirection;
    private bool isDashing;


    public void Enter(PlayerStateMachine ctx)
    {
        if (!ctx.CanDash())
        {
            ctx.SwitchState(ctx.IdleState);
            return;
        }

        this.ctx = ctx;
        input = this.ctx.Input;
        animator = this.ctx.Animator;
        rb = this.ctx.Rb;
        data = this.ctx.Data;

        //does not exist right now
        //animator.setTrigger("Dash")
        
        StartDash();
    }

    public void Exit(PlayerStateMachine ctx)
    {
        isDashing = false;
    }

    public void UpdateFixed(PlayerStateMachine ctx)
    {
        
    }

    public void Update(PlayerStateMachine ctx)
    {
        if (!isDashing) return;

        if (Time.time >= dashStartTime + data.dashDuration)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        isDashing = false;


        Vector3 moveDir = input.MoveInputNormalized;
        float maxMoveSpeed = data.moveSpeed;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = moveDir * maxMoveSpeed;
            ctx.SwitchState(ctx.MovingState);
        }
        else
        {
            ctx.SwitchState(ctx.IdleState);
        }
    }

    private void StartDash()
    {
        ctx.ConsumeDash();

        dashStartTime = Time.time;
        isDashing = true;

        if(input.MoveInputNormalized.sqrMagnitude > 0.01f)
        {
            dashDirection = input.MoveInputNormalized;
        }
        else
        {
            dashDirection = ctx.PlayerFlipper.isFacingRight ? Vector3.right : Vector3.left;
        }
        

        dashDirection.Normalize();

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dashDirection * data.dashForce, ForceMode.Impulse);
    }

    public override string ToString()
    {
        return "Dash";
    }
}
