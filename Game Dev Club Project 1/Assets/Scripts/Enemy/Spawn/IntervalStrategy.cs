using UnityEngine;

[CreateAssetMenu(fileName = "IntervalStrategy", menuName = "Enemy/IntervalStrategy")]
public class IntervalStrategy : SpawnStrategy
{
    [SerializeField] private float spawnInterval = 4f;

    private float lastSpawnTime;
    private int counter = 0;

    public override bool TrySpawn(EnemySpawner spawner, EnemyType[] enemyTypes)
    {
        if (Time.time - lastSpawnTime < spawnInterval)
            return false;
        
        
        spawner
            .Create(spawner.transform.position, enemyTypes[counter % enemyTypes.Length])
            .WithPatrolCenter(spawner.transform.position)
            .Build();

        counter++;
        lastSpawnTime = Time.time;
        return true;
    }
}
