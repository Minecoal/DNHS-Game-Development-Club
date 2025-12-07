using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyContext context);
    void Exit(EnemyContext context);
    void Tick(EnemyContext context, float deltaTime);
    void FixedTick(EnemyContext context, float fixedDeltaTime);
}
