using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private float idleTimer;
    private float idleDuration; // time to wait before patrolling

    public void Enter(EnemyContext context)
    {
        idleTimer = 0f;
        idleDuration = Random.Range(1f, 5f);
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

        idleTimer += deltaTime;
        if (idleTimer >= idleDuration)
        {
            context.StateMachine.ChangeState(new EnemyPatrolState(), context);
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
