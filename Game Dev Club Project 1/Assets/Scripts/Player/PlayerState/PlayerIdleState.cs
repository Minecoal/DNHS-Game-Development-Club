using UnityEngine;

public class PlayerIdleState : PlayerLocomotionState
{
    public override void Enter(PlayerContext context)
    {
        base.Enter(context);
        context.AnimationManager.PlayAnimation(context.AnimationManager.Idle);
    }

    public override void FixedTick(PlayerContext context, float fixedDeltaTime)
    {
        base.FixedTick(context, fixedDeltaTime);
        if (context.Input.MoveInputNormalized.sqrMagnitude > 0.01f)
        {
            context.StateMachine.ChangeState(new PlayerMovingState(), context);
            return;
        }
    }

    public override string ToString()
    {
        return "Idle";
    }
}