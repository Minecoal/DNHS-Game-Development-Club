using UnityEngine;

public interface IEnemyState
{
    void Enter(Enemy enemy);
    void Exit(Enemy enemy);
    void Tick(Enemy enemy, float deltaTime);
    void FixedTick(Enemy enemy, float fixedDeltaTime);
}
