using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    PlayerStateManager ctx;
    PlayerInputHandler input;
    PlayerData data;
    Rigidbody2D rb;
    Animator animator;

    public void Enter(PlayerStateManager ctx)
    {
        this.ctx = ctx;
        input = this.ctx.Input;
        data = this.ctx.Data;
        rb = this.ctx.Rb;
        animator = this.ctx.Animator;

        animator?.SetTrigger("EnterIdle");
        input.AttackPressed += OnAttack;
    }

    public void Exit(PlayerStateManager ctx)
    {
        input.AttackPressed -= OnAttack;
    }

    public void UpdateFixed(PlayerStateManager ctx)
    {
        
        if (input.MoveInput.sqrMagnitude > 0.01f)
        {
            ctx.SwitchState(ctx.MovingState);
            return;
        }
        Decelerate();
    }

    public void Update(PlayerStateManager ctx)
    {

    }

    public void Decelerate()
    {
        Vector2 movement = new Vector2(-rb.linearVelocity.x * data.decelAmount, -rb.linearVelocity.y * data.decelAmount);
        rb.AddForce(movement.x * Vector2.right, ForceMode2D.Force);
        rb.AddForce(movement.y * Vector2.up, ForceMode2D.Force);
    }

    private void OnAttack()
    {
        ctx.SwitchState(ctx.AttackState);
    }

    public override string ToString()
    {
        return "Idle";
    }
}