using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMovingState : IPlayerState
{
    PlayerStateManager ctx;
    Transform player;
    PlayerInputHandler input;
    PlayerData data;
    Rigidbody2D rb;
    Animator animator;

    public void Enter(PlayerStateManager ctx)
    {
        // ensure animator trigger uses the MovementController available from PlayerManager when possible
        this.ctx = ctx;
        player = this.ctx.Player;
        input = this.ctx.Input;
        data = this.ctx.Data;
        rb = this.ctx.Rb;
        animator = this.ctx.Animator;

        animator?.SetTrigger("EnterWalk");
        input.AttackPressed += OnAttack;
    }

    public void Exit(PlayerStateManager ctx)
    {
        input.AttackPressed -= OnAttack;
    }

    public void UpdateFixed(PlayerStateManager ctx)
    {
        if (input.MoveInput.sqrMagnitude <= 0.01f)
        {
            ctx.SwitchState(ctx.IdleState);
            return;
        }
        ApplyMovement(input.MoveInput, player, rb, data, Time.fixedDeltaTime);
    }

    public void Update(PlayerStateManager ctx)
    {

    }

    public void ApplyMovement(Vector2 input, Transform player, Rigidbody2D rb, PlayerData data, float deltaTime)
    {
        Vector2 targetSpeed = new Vector2(input.x * data.moveSpeed, input.y * data.moveSpeed);

        Debug.DrawLine((Vector2)player.transform.position, (Vector2)player.transform.position + targetSpeed, Color.green);
        Debug.DrawLine((Vector2)player.transform.position, (Vector2)player.transform.position + rb.linearVelocity, Color.yellow);

        //calculate accel/deccel
        Vector2 accelRate = new Vector2();
        accelRate.x = (Mathf.Abs(targetSpeed.x) > 0.01f) ? data.accelAmount : data.decelAmount;
        accelRate.y = (Mathf.Abs(targetSpeed.y) > 0.01f) ? data.accelAmount : data.decelAmount;

        //conserve momentum
        if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed.x) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed.x) && Mathf.Abs(targetSpeed.x) > 0.01f)
        {
            accelRate.x = 0;
        }
        if (Mathf.Abs(rb.linearVelocity.y) > Mathf.Abs(targetSpeed.y) && Mathf.Sign(rb.linearVelocity.y) == Mathf.Sign(targetSpeed.y) && Mathf.Abs(targetSpeed.y) > 0.01f)
        {
            accelRate.y = 0;
        }

        Vector2 speedDiff = new Vector2(targetSpeed.x - rb.linearVelocity.x, targetSpeed.y - rb.linearVelocity.y);
        Vector2 movement = new Vector2(speedDiff.x * accelRate.x, speedDiff.y * accelRate.y);

        rb.AddForce(movement.x * Vector2.right, ForceMode2D.Force);
        rb.AddForce(movement.y * Vector2.up, ForceMode2D.Force);
    }

    private void OnAttack()
    {
        ctx.SwitchState(ctx.AttackState);
    }

    public override string ToString()
    {
        return "Moving";
    }
}