using UnityEngine;

public abstract class EnemyAIBase : MonoBehaviour
{
    protected Enemy enemy;

    protected virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null) Debug.LogError("EnemyAIBase requires an Enemy component on the same GameObject.");
    }

    protected virtual void Update()
    {
        if (enemy == null) return;
        enemy.StateMachine.Tick(enemy, Time.deltaTime);
    }

    public abstract IEnemyState GetInitialState();
}
