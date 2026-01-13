using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    PlayerContext playerContext;

    private bool hasBufferedInput = false; //input buffering
    private const float BUFFER_TIME = 0.25f;
    private float bufferTimestamp = 0f;

    private bool canSwitchState = false;

    public void Enter(PlayerContext context)
    {
        canSwitchState = false;
        playerContext = context;
        playerContext.ActiveWeapon.OnEnableSwitchState += EnableSwitchState;
        context.Input.OnAttack += OnAttack;
        BufferInput();
    }

    public void Exit(PlayerContext context)
    {
        playerContext.ActiveWeapon.OnEnableSwitchState -= EnableSwitchState;
        context.Input.OnAttack -= OnAttack;
        hasBufferedInput = false;
    }

    public void FixedTick(PlayerContext context, float fixedDeltaTime)
    {
        
    }

    public void Tick(PlayerContext context, float deltaTime)
    {
        TryStartAttack();        

        if (!canSwitchState) return;
        if (context.Input.MoveInputNormalized.sqrMagnitude > 0.01f) {
            context.StateMachine.ChangeState(new PlayerMovingState(), context);
        } else {
            context.StateMachine.ChangeState(new PlayerIdleState(), context);
        }
    }

    private void OnAttack()
    {
        BufferInput();
    }

    public void BufferInput()
    {
        hasBufferedInput = true;
        bufferTimestamp = Time.time;
    }

    private void TryStartAttack()
    {
        if (!hasBufferedInput) return;

        if (Time.time - bufferTimestamp > BUFFER_TIME) // clear buffer after a period of time
        {
            hasBufferedInput = false;
            return;
        }

        if (playerContext.ActiveWeapon.TryAttack(playerContext)) // clear buffer after performing an attack successfully
        {
            canSwitchState = false;
            hasBufferedInput = false;
        }
    }

    private void EnableSwitchState()
    {
        canSwitchState = true;
    }

    public override string ToString()
    {
        return "Attack";
    }
}