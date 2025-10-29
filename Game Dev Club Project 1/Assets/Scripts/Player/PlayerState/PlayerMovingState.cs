using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMovingState : IPlayerState
{
    PlayerStateManager ctx;
    Transform player;
    PlayerInputHandler input;
    PlayerData data;
    Rigidbody rb;
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
        ApplyMovement(input.MoveInput, player, rb, data);
    }

    public void Update(PlayerStateManager ctx)
    {

    }

    public void ApplyMovement(Vector3 input, Transform player, Rigidbody rb, PlayerData data)
    {
        Vector3 targetSpeed = new Vector3(input.x * data.moveSpeed, 0f, input.z * data.moveSpeed);

        Debug.DrawLine(player.transform.position, player.transform.position + targetSpeed, Color.green);
        Debug.DrawLine(player.transform.position, player.transform.position + rb.linearVelocity, Color.yellow);

        //calculate accel/deccel
        Vector3 accelRate = new Vector3();
        accelRate.x = (Mathf.Abs(targetSpeed.x) > 0.01f) ? data.accelAmount : data.decelAmount;
        accelRate.y = 0f;
        accelRate.z = (Mathf.Abs(targetSpeed.z) > 0.01f) ? data.accelAmount : data.decelAmount;

        //conserve momentum
        if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed.x) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed.x) && Mathf.Abs(targetSpeed.x) > 0.01f)
        {
            accelRate.x = 0;
        }
        if (Mathf.Abs(rb.linearVelocity.z) > Mathf.Abs(targetSpeed.z) && Mathf.Sign(rb.linearVelocity.z) == Mathf.Sign(targetSpeed.z) && Mathf.Abs(targetSpeed.z) > 0.01f)
        {
            accelRate.z = 0;
        }

        Vector3 speedDiff = new Vector3(targetSpeed.x - rb.linearVelocity.x, 0f, targetSpeed.z - rb.linearVelocity.z);
        Vector3 movement = new Vector3(speedDiff.x * accelRate.x, 0f, speedDiff.z * accelRate.z);

        rb.AddForce(movement.x * Vector3.right, ForceMode.Force);
        rb.AddForce(movement.z * Vector3.forward, ForceMode.Force);
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