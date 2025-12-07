using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    public void Enter(EnemyContext context)
    {
        //play aggro animation  
    }

    public void Exit(EnemyContext context)
    {
        //play deaggro animation
    }

    public void Tick(EnemyContext context, float deltaTime)
    {
        if (!context.Enemy.IsPlayerInChaseRange())
        {
            context.StateMachine.ChangeState(new EnemyIdleState(), context);
            return;
        }

        // for now just stop and log
        if (context.Enemy.IsPlayerInAttackRange())
        {
            // TODO: Change to an AttackState that handles animation/hitbox timing
            Debug.Log("Enemy in attack range");
        }

    }

    public void FixedTick(EnemyContext context, float fixedDeltaTime)
    {
        context.Enemy.MoveTowardsPosition(context.Target.position, context.EnemyData.speed, context.EnemyData.accelAmount, context.EnemyData.decelAmount);
    }

    public override string ToString()
    {
        return "Chase";
    }
}
