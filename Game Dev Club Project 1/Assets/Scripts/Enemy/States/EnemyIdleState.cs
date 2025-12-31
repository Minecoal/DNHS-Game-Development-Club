public class EnemyIdleState : IEnemyState
{
    public void Enter(EnemyContext context)
    {
        //play idle animation here
    }

    public void Exit(EnemyContext context)
    {

    }

    public void Tick(EnemyContext context, float deltaTime)
    {
        // If player in range, chase. Otherwise, if there are patrol points, go to patrol.
        if (context.Enemy.IsPlayerInDetectionRange())
        {
            context.StateMachine.ChangeState(new EnemyChaseState(), context);
            return;
        }

        // if (context.Enemy. != null && context.Enemy..Length > 0)
        // {
        //     context.StateMachine.ChangeState(new EnemyPatrolState(), context);
        //     return;
        // }
    }

    public void FixedTick(EnemyContext context, float fixedDeltaTime)
    {
        
    }
    
    public override string ToString()
    {
        return "Idle";
    }
}
