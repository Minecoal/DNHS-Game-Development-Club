using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    PlayerStateMachine ctx;
    PlayerInputHandler input;
    PlayerData data;
    Rigidbody rb;
    Animator animator;

    public void Enter(PlayerStateMachine ctx)
    {
        this.ctx = ctx;
        input = this.ctx.Input;
        data = this.ctx.Data;
        rb = this.ctx.Rb;
        animator = this.ctx.Animator;

        animator?.SetTrigger("EnterIdle");
        input.AttackPressed += OnAttack;
    }

    public void Exit(PlayerStateMachine ctx)
    {
        input.AttackPressed -= OnAttack;
    }

    public void UpdateFixed(PlayerStateMachine ctx)
    {
        
        if (input.MoveInput.sqrMagnitude > 0.01f)
        {
            ctx.SwitchState(ctx.MovingState);
            return;
        }
        Decelerate();
    }

    public void Update(PlayerStateMachine ctx)
    {

    }

    public void Decelerate()
    {
        Vector3 movement = new Vector3(-rb.linearVelocity.x * data.decelAmount, 0f, -rb.linearVelocity.z * data.decelAmount);
        rb.AddForce(movement.x * Vector3.right, ForceMode.Force);
        rb.AddForce(movement.y * Vector3.up, ForceMode.Force);
        rb.AddForce(movement.z * Vector3.forward, ForceMode.Force);
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