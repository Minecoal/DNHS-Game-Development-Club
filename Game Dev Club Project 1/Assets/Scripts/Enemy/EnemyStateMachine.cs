using UnityEngine;

public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void Initialize(IEnemyState startingState, EnemyContext context)
    {
        currentState = startingState;
        currentState?.Enter(context);
    }

    public void ChangeState(IEnemyState newState, EnemyContext context)
    {
        if (currentState == newState) return;
        currentState?.Exit(context);
        currentState = newState;
        currentState?.Enter(context);
    }

    public void Tick(EnemyContext context, float deltaTime)
    {
        currentState?.Tick(context, deltaTime);
    }

    public void FixedTick(EnemyContext context, float fixedDeltaTime)
    {
        currentState?.FixedTick(context, fixedDeltaTime);
    }

    public string GetState()
    {
        return currentState.ToString();
    }
}
