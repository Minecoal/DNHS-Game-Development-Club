using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    public void Enter(Enemy enemy) { }
    public void Exit(Enemy enemy) { }

    public void Tick(Enemy enemy, float deltaTime)
    {
        if (!enemy.IsPlayerInDetectionRange())
        {
            enemy.StateMachine.ChangeState(new EnemyIdleState(), enemy);
            return;
        }

        Transform player = enemy.GetPlayerTransform();
        if (player == null) return;

        Vector3 dir = (player.position - enemy.transform.position).normalized;
        float speed = 2f; // TODO: expose via enemy or state
        enemy.transform.position += dir * speed * deltaTime;

        if (enemy.IsPlayerInAttackRange())
        {
            // TODO: change to attack state
        }
    }
}
