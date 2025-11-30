using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private int currentIndex = 0;

    public void Enter(Enemy enemy)
    {
        // pick the closest patrol point as starting index
        currentIndex = 0;
        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0) return;

        float best = float.MaxValue;
        for (int i = 0; i < enemy.patrolPoints.Length; i++)
        {
            float d = Vector3.SqrMagnitude(enemy.patrolPoints[i].position - enemy.transform.position);
            if (d < best)
            {
                best = d;
                currentIndex = i;
            }
        }
    }

    public void Exit(Enemy enemy)
    {
        // nothing to clean up for now
    }

    public void Tick(Enemy enemy, float deltaTime)
    {
        // If player detected, switch to chase
        if (enemy.IsPlayerInDetectionRange())
        {
            enemy.StateMachine.ChangeState(new EnemyChaseState(), enemy);
            return;
        }

        if (enemy.patrolPoints == null || enemy.patrolPoints.Length == 0)
        {
            enemy.StateMachine.ChangeState(new EnemyIdleState(), enemy);
            return;
        }

        Transform target = enemy.patrolPoints[currentIndex];
        if (enemy.Pathfinder != null)
        {
            Vector3 dir = enemy.Pathfinder.ComputeDirectionTowards(enemy.transform.position, target.position);
            if (dir.sqrMagnitude > 0.001f)
            {
                enemy.transform.position += dir * 2f * deltaTime;
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(dir), 10f * deltaTime);
            }
        }
        else
        {
            enemy.MoveTowardsPosition(target.position, 2f, deltaTime);
        }

        float dist = Vector3.Distance(enemy.transform.position, target.position);
        if (dist < 0.25f)
        {
            currentIndex = (currentIndex + 1) % enemy.patrolPoints.Length;
        }
    }

    public void FixedTick(Enemy enemy, float fixedDeltaTime)
    {

    }
    
    public override string ToString()
    {
        return "Patrol";
    }
}
