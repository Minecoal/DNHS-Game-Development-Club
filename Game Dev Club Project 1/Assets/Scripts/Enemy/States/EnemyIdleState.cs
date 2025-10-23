using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        // maybe play idle anim
    }

    public void Exit(Enemy enemy)
    {
    }

    public void Tick(Enemy enemy, float deltaTime)
    {
        // transition to patrol or chase
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
}
