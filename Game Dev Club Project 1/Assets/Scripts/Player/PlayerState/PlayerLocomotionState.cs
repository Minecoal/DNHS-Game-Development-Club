using UnityEngine;
// do not create instance of this class
public class PlayerLocomotionState : IPlayerState
{
    public virtual void Enter(PlayerContext context)
    {
        context.PlayerFlipper.CanFlip(true);
    }
    public virtual void Exit(PlayerContext context) {}

    public virtual void FixedTick(PlayerContext context, float fixedDeltaTime)
    {
        //override and implement movement logic
    }

    public virtual void Tick(PlayerContext context, float deltaTime)
{
    if (context.Input.ConsumeDash() && context.Stamina.CanUseStamina(context.Data.dashStaminaCost))
    {
        context.StateMachine.ChangeState(new PlayerDashState(), context);
        return;
    }

    if (context.Input.ConsumePrimaryAttack())
    {
        if (context.ActivePrimaryWeapon != null && context.ActivePrimaryWeapon.CanAttack(context, false))
            context.StateMachine.ChangeState(new PlayerAttackState(), context);
        return;
    }
    
    if (context.Input.ConsumeSecondaryAttack())
    {
        if (context.ActiveSecondaryWeapon != null && context.ActiveSecondaryWeapon.CanAttack(context, false))
            context.StateMachine.ChangeState(new PlayerSecondaryAttackState(), context);
        return;
    }
}

    public override string ToString()
    {
        return "Locomotion";
    }
}