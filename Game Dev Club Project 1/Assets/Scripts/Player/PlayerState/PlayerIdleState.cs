using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    MovementLogic movement;
    PlayerInputHandler input;

    public void Enter(PlayerStateManager ctx)
    {
        movement = ctx.Movement;
        input = ctx.Input;
        ctx.Anim?.SetTrigger("EnterIdle");
    }

    public void Exit(PlayerStateManager ctx)
    {
        
    }

    public void UpdateFixed(PlayerStateManager ctx)
    {
        
        if (input.MoveInput.sqrMagnitude > 0.01f)
        {
            ctx.SwitchState(ctx.MovingState);
            return;
        }
        movement.Decelerate();
    }

    public void Update(PlayerStateManager ctx)
    {
        
    }
}