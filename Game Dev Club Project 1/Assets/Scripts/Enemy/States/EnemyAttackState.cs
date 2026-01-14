using UnityEngine;

public class EnemyAttackState : IEnemyState
{

    public void Enter(EnemyContext context)
    {
        //play attack animation here
    }

    public void Exit(EnemyContext context)
    {

    }

    public void Tick(EnemyContext context, float deltaTime)
    {
        if (context.Enemy.IsPlayerInAttackRange()){
            context.StateMachine.ChangeState(new EnemyAttackState(), context);
            return;
        }
        if (context.Enemy.IsPlayerInDetectionRange())
        {
            context.StateMachine.ChangeState(new EnemyChaseState(), context);
            return;
        } else {
            context.StateMachine.ChangeState(new EnemyIdleState(), context);
        }
    }

    public void FixedTick(EnemyContext context, float fixedDeltaTime)
    {
        
    }
    
    public override string ToString()
    {
        return "Idle";
    }
}
