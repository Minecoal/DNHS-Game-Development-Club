using UnityEngine;

public class PlayerDashAttackState : IPlayerState
{
    private bool canSwitchState = false;

    public void Enter(PlayerContext context)
    {
        canSwitchState = false;
        context.ActivePrimaryWeapon.TryAttack(context, true);
        context.ActivePrimaryWeapon.OnEnableSwitchState += EnableSwitchState;

        context.PlayerFlipper.CanFlip(false);
    }

    public void Exit(PlayerContext context)
    {
        context.ActivePrimaryWeapon.OnEnableSwitchState -= EnableSwitchState;
    }

    public void FixedTick(PlayerContext context, float fixedDeltaTime)
    {
        
    }

    public void Tick(PlayerContext context, float deltaTime)
    {
        if (!canSwitchState) return;
        // transition to appropriate locomotion state
        if (context.Input.MoveInputNormalized.sqrMagnitude > 0.01f)
            context.StateMachine.ChangeState(new PlayerMovingState(), context);
        else
            context.StateMachine.ChangeState(new PlayerIdleState(), context);
    }

    private void EnableSwitchState()
    {
        canSwitchState = true;
    }

    public override string ToString()
    {
        return "DashAttack";
    }
}