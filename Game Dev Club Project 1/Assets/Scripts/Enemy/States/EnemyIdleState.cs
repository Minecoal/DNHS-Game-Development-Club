using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        //play idle animation here
    }

    public void Exit(Enemy enemy)
    {

    }

    public void Tick(Enemy enemy, float deltaTime)
    {
        // If player in range, chase. Otherwise, if there are patrol points, go to patrol.
        if (enemy.IsPlayerInDetectionRange())
        {
            enemy.StateMachine.ChangeState(new EnemyChaseState(), enemy);
            return;
        }

        if (enemy.patrolPoints != null && enemy.patrolPoints.Length > 0)
        {
            enemy.StateMachine.ChangeState(new EnemyPatrolState(), enemy);
            return;
        }
    }

    public void FixedTick(Enemy enemy, float fixedDeltaTime)
    {

    }
    
        public override string ToString()
    {
        return "Idle";
    }
}
