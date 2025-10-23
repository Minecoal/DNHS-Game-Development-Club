using UnityEngine;

public class PlayerMovingState : IPlayerState
{
    PlayerInputHandler input;
    MovementLogic movement;
    public void Enter(PlayerStateManager ctx)
    {
        // ensure animator trigger uses the MovementController available from PlayerManager when possible
        input = ctx.Input;
        movement = ctx.Movement;
        ctx.Anim?.SetTrigger("EnterWalk");
    }

    public void Exit(PlayerStateManager ctx)
    {
    }

    public void UpdateFixed(PlayerStateManager ctx)
    {
        if (input.MoveInput.sqrMagnitude <= 0.01f)
        {
            ctx.SwitchState(ctx.IdleState);
            return;
        }
        movement.ApplyMovement(input.MoveInput, Time.fixedDeltaTime);
    }

    public void Update(PlayerStateManager ctx)
    {
        
    }
}