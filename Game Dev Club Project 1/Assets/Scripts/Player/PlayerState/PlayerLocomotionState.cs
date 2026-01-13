using UnityEngine;
// do not create instance of this class
public class PlayerLocomotionState : IPlayerState
{
    PlayerContext playerContext;

    public virtual void Enter(PlayerContext context)
    {
        playerContext = context;
        context.Input.OnAttack += OnAttack;
    }

    public virtual void Exit(PlayerContext context)
    {
        context.Input.OnAttack -= OnAttack;
    }

    public virtual void FixedTick(PlayerContext context, float fixedDeltaTime)
    {
        //override and implement movement logic
    }

    public virtual void Tick(PlayerContext context, float DeltaTime)
    {
        
    }

    private void OnAttack()
    {
        playerContext.StateMachine.ChangeState(new PlayerAttackState(), playerContext);
    }

    public override string ToString()
    {
        return "Locomotion";
    }
}