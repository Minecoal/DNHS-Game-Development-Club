using UnityEngine;

[CreateAssetMenu(fileName = "OneTimeStrategy", menuName = "Enemy/OneTimeStrategy")]
public class OneTimeStrategy : SpawnStrategy
{
    private bool hasSpawned = false;

    public override bool TrySpawn(EnemySpawner spawner, EnemyType[] enemyTypes)
    {
        if (hasSpawned) return false;
        if (enemyTypes == null) return false;

        foreach(EnemyType enemyType in enemyTypes){
            spawner
                .Create(spawner.transform.position, enemyType)
                .WithPatrolCenter(spawner.transform.position)
                .Build();
        }
        hasSpawned = true;
        return true;
    }
}
