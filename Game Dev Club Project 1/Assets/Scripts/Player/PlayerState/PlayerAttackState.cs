using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    PlayerInputHandler input;
    public void Enter(PlayerStateManager ctx)
    {
        input = ctx.Input;
        // trigger the attack via the PlayerManager's AttackController when possible
        AttackLogic attack = ctx.Attack;
        if (attack != null)
        {
            attack.TryStartAttack(0);
        }
    }

    public void Exit(PlayerStateManager ctx)
    {
        
    }

    public void UpdateFixed(PlayerStateManager ctx)
    {

    }

    public void Update(PlayerStateManager ctx)
    {
        if (input.MoveInput.sqrMagnitude > 0.01f)
        {
            ctx.SwitchState(ctx.MovingState);
        }
        else
        {
            ctx.SwitchState(ctx.IdleState);
        }
    }
}