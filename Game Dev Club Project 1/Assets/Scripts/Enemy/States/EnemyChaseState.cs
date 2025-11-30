using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        //play aggro animation  
    }

    public void Exit(Enemy enemy)
    {
        //play deaggro animation
    }

    public void Tick(Enemy enemy, float deltaTime)
    {
        Transform player = enemy.GetPlayerTransform();
        if (player == null)
        {
            enemy.StateMachine.ChangeState(new EnemyPatrolState(), enemy);
            return;
        }

        if (!enemy.IsPlayerInChaseRange())
        {
            enemy.StateMachine.ChangeState(new EnemyPatrolState(), enemy);
            return;
        }

        // Move towards the player (use injected Pathfinder when available)
        if (enemy.Pathfinder != null)
        {
            Vector3 dir = enemy.Pathfinder.ComputeDirectionTowards(enemy.transform.position, player.position);
            if (dir.sqrMagnitude > 0.001f)
            {
                enemy.transform.position += dir * 3.5f * deltaTime;
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(dir), 10f * deltaTime);
            }
        }
        else
        {
            enemy.MoveTowardsPosition(player.position, 3.5f, deltaTime);
        }

        // for now just stop and log
        if (enemy.IsPlayerInAttackRange())
        {
            // TODO: Change to an AttackState that handles animation/hitbox timing
            Debug.Log("Enemy in attack range");
        }
    }

    public void FixedTick(Enemy enemy, float fixedDeltaTime)
    {

    }

    public override string ToString()
    {
        return "Chase";
    }
}
