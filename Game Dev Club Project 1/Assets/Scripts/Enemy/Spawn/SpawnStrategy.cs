using UnityEngine;

public abstract class SpawnStrategy : ScriptableObject
{
    public abstract bool TrySpawn(EnemySpawner spawner, EnemyType[] enemyTypes);
}
