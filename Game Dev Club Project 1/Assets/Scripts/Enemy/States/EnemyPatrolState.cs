using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private int currentIndex = 0;

    public void Enter(Enemy enemy)
    {
        currentIndex = 0;
    }

    public void Exit(Enemy enemy)
    {
    }

    public void Tick(Enemy enemy, float deltaTime)
    {
        if (enemy.IsPlayerInDetectionRange())
        {
            enemy.StateMachine.ChangeState(new EnemyChaseState(), enemy);
            return;
        }

        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0) return;

        Transform target = enemy.patrolPoints[currentIndex];
        Vector3 dir = (target.position - enemy.transform.position);
        float dist = dir.magnitude;
        if (dist < 0.1f)
        {
            currentIndex = (currentIndex + 1) % enemy.patrolPoints.Length;
            return;
        }

        dir.Normalize();
        enemy.transform.position += dir * (enemy.GetComponent<Rigidbody>()?.linearVelocity.magnitude ?? 1f) * deltaTime; // naive move, replace with NPC movement
    }
}
