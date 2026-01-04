using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private Vector3 targetPatrolPos;
    private float patrolSpeedMultiplier = 0.5f;
    private float patrolRadius = 5f;

    //stuck related
    private Vector3 lastPosition;
    private float stuckTime = 0f;
    private float checkInterval = 0.5f;
    private float maxStuckDuration = 2f; // how long to be stuck before acting
    private float minMoveDistance = 0.1f; // minimum movement to be considered "moving"


    public void Enter(EnemyContext context)
    {
        SetNewPatrolPos(context);
    }

    public void Exit(EnemyContext context)
    {
        
    }

    public void Tick(EnemyContext context, float deltaTime)
    {
        if (context.Enemy.IsPlayerInDetectionRange())
        {
            context.StateMachine.ChangeState(new EnemyChaseState(), context);
            return;
        }

        // If reached patrol point, go back to idle
        if (Vector3.Distance(context.Enemy.transform.position, targetPatrolPos) < 0.5f)
        {
            context.StateMachine.ChangeState(new EnemyIdleState(), context);
        }

        StuckCheck(context, deltaTime);
    }

    public void FixedTick(EnemyContext context, float fixedDeltaTime)
    {
        context.Enemy.MoveTowardsPosition(targetPatrolPos, context.EnemyData.speed * patrolSpeedMultiplier, context.EnemyData.accelAmount * 0.3f, context.EnemyData.decelAmount * 0.5f);
    }
    
    public override string ToString()
    {
        return "Patrol";
    }

    private void SetNewPatrolPos(EnemyContext context) // set patrol point
    {
        Vector2 randCircle = Random.insideUnitCircle * patrolRadius;
        targetPatrolPos = context.PatrolCenter + new Vector3(randCircle.x, 0, randCircle.y);
        targetPatrolPos = context.Pathfinder.SampleOnNavMesh(targetPatrolPos);
    }

    private void StuckCheck(EnemyContext context, float deltaTime)
    {
        stuckTime += deltaTime;
        if (stuckTime < checkInterval) return;

        Vector3 currentPos = context.Enemy.transform.position;
        float distanceMoved = Vector3.Distance(currentPos, lastPosition);

        if (distanceMoved < minMoveDistance)
        {
            SetNewPatrolPos(context);
        }

        lastPosition = currentPos;
        stuckTime = 0f;
    }
}
