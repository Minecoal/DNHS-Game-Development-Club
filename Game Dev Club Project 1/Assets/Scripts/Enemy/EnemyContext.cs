using UnityEngine;

public class EnemyContext
{
    public EnemyStateMachine StateMachine;
    public Enemy Enemy;
    public Rigidbody Rigidbody;
    public Health Health;
    public Transform Target;
    public IPathfinder Pathfinder;
    public EnemyData EnemyData;
    public Vector3 PatrolCenter;

    public EnemyContext(
        EnemyStateMachine stateMachine,
        Enemy enemy, 
        Rigidbody rigidbody, 
        Health health, 
        Transform target, 
        IPathfinder pathfinder, 
        EnemyData enemyData,
        Vector3 patrolCenter)
    {
        StateMachine = stateMachine;
        Enemy = enemy;
        Rigidbody = rigidbody;
        Health = health;
        Target = target;
        Pathfinder = pathfinder;
        EnemyData = enemyData;
        PatrolCenter = patrolCenter;
    }
}
