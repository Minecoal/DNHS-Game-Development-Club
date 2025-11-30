using UnityEngine;

public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void Initialize(IEnemyState startingState, Enemy owner)
    {
        currentState = startingState;
        currentState?.Enter(owner);
    }

    public void ChangeState(IEnemyState newState, Enemy owner)
    {
        if (currentState == newState) return;
        currentState?.Exit(owner);
        currentState = newState;
        currentState?.Enter(owner);
    }

    public void Tick(Enemy owner, float deltaTime)
    {
        currentState?.Tick(owner, deltaTime);
    }

    public void FixedTick(Enemy owner, float fixedDeltaTime)
    {
        currentState?.FixedTick(owner, fixedDeltaTime);
    }

    public string GetState()
    {
        return currentState.ToString();
    }
}
