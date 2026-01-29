using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPrefabEntry[] enemyPrefabs;
    private Dictionary<EnemyType, GameObject> enemyPrefabsDict;
    private EnemyType[] enemyTypes;

    [SerializeField] private SpawnStrategy[] strategies;
    private SpawnStrategy[] runtimeStrategies;

    [SerializeField] private int maxSpawnCount;
    [SerializeField] private int spawnCount;

    private void Awake()
    {
        enemyPrefabsDict = new Dictionary<EnemyType, GameObject>();

        foreach (var entry in enemyPrefabs)
            enemyPrefabsDict[entry.type] = entry.prefab;

        enemyTypes = enemyPrefabsDict.Keys.ToArray();

        //clone runtime strategy
        runtimeStrategies = new SpawnStrategy[strategies.Length];
        for (int i = 0; i < strategies.Length; i++)
        {
            runtimeStrategies[i] = Instantiate(strategies[i]);
        }
    }

    void Update()
    {
        if (spawnCount >= maxSpawnCount) return;
        foreach (SpawnStrategy strategy in runtimeStrategies){
            if (strategy.TrySpawn(this, enemyTypes)){
                IncrementSpawnCount();   
            }
        }
    }
    public EnemyBuilder Create(Vector3 spawnPoint, EnemyType type)
    {
        if (spawnCount >= maxSpawnCount) return null;
        if (!enemyPrefabsDict.TryGetValue(type, out GameObject prefab)){
            Debug.LogError("No Prefab Found in Dictionary");
            return null;
        }
        return new EnemyBuilder(this, spawnPoint, prefab);
    }

    public void DecrementSpawnCount(){
        spawnCount = Mathf.Max(spawnCount - 1, 0);
    }

    public void IncrementSpawnCount(){
        spawnCount = Mathf.Min(spawnCount + 1, maxSpawnCount);
    }

    public class EnemyBuilder
    {
        private EnemySpawner spawner;
        private Vector3 spawnPoint = Vector3.zero;
        private Vector3 patrolCenter = Vector3.zero;
        private GameObject prefab;

        public EnemyBuilder(EnemySpawner spawner, Vector3 spawnPoint, GameObject prefab)
        {
            this.spawner = spawner;
            this.spawnPoint = spawnPoint;
            this.patrolCenter = spawnPoint; // patrols around spawnpoint by default
            if (prefab == null){
                Debug.LogError("No Prefab Found");
            }
            this.prefab = prefab;
        }

        public EnemyBuilder WithPatrolCenter(Vector3 center)
        {
            patrolCenter = center;
            return this;
        }

        public GameObject Build()
        {
            Vector3 finalPos = spawnPoint + prefab.transform.localPosition; // addes prefab offset
            GameObject obj = Instantiate(prefab, finalPos, Quaternion.identity);
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy == null) enemy = obj.GetComponentInChildren<Enemy>();
            if (enemy == null){
                Debug.LogError("Cannot Find Enemy Component");
                return null;
            }
            enemy.SetPatrolCenter(patrolCenter);
            enemy.Initialize();
            enemy.Health.OnDied += spawner.DecrementSpawnCount;
            return obj;
        }
    }
}

[System.Serializable]
public class EnemyPrefabEntry
{
    public EnemyType type;
    public GameObject prefab;
}

