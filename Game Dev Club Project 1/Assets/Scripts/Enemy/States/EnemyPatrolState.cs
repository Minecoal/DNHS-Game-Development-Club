using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private int currentIndex = 0;

    public void Enter(EnemyContext context)
    {

    }

    public void Exit(EnemyContext context)
    {
        
    }

    public void Tick(EnemyContext context, float deltaTime)
    {

    }

    public void FixedTick(EnemyContext context, float fixedDeltaTime)
    {

    }
    
    public override string ToString()
    {
        return "Patrol";
    }
}
