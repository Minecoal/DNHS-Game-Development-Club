using UnityEngine;

public class PlayerMovingState : PlayerLocomotionState
{
    public override void Enter(PlayerContext context)
    {
        base.Enter(context);
        context.AnimationManager.PlayAnimation(PlayerAnimationManager.WalkHash);
    }

    public override void FixedTick(PlayerContext context, float fixedDeltaTime)
    {
        base.FixedTick(context, fixedDeltaTime);
        if (context.Input.MoveInputNormalized.sqrMagnitude <= 0.01f)
        {
            context.StateMachine.ChangeState(new PlayerIdleState(), context);
            return;
        }
        context.Player.ApplyMovement(context.Input.MoveInputNormalized, context.PlayerGO.transform, context.Rb, context.Data);
    }
    
    public override string ToString()
    {
        return "Moving";
    }
}