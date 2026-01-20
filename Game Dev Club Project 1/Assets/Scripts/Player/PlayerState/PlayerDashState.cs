using UnityEngine;
using System;

public class PlayerDashState : IPlayerState
{
    private float dashStartTime;
    private Vector3 dashDirection;

    public void Enter(PlayerContext context)
    {
        StartDash(context);
    }

    public void Exit(PlayerContext context) {}

    public void Tick(PlayerContext context, float deltaTime)
    {
        if (Time.time < dashStartTime + context.Data.dashDuration)
            return;

        // Dash finished
        if (context.Input.MoveInputNormalized.sqrMagnitude > 0.01f)
            context.StateMachine.ChangeState(new PlayerMovingState(), context);
        else
            context.StateMachine.ChangeState(new PlayerIdleState(), context);
    }

    public void FixedTick(PlayerContext context, float fixedDeltaTime){}

    private void StartDash(PlayerContext context)
    {
        //does not exist right now
        // context.AnimationManager.PlayAnimation(PlayerAnimationManager.DashHash);

        dashStartTime = Time.time;
        context.Player.ApplyForce(context.Data.dashForce, context);
    }

    public override string ToString()
    {
        return "Dash";
    }
}
