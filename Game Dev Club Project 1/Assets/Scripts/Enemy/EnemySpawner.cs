using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPrefabEntry[] enemyPrefabs;
    private Dictionary<EnemyType, GameObject> enemyPrefabsDict;
    [SerializeField] float spawnInterval = 4f;
    private float timer = 0f;

    private void Awake()
    {
        enemyPrefabsDict = new Dictionary<EnemyType, GameObject>();

        foreach (var entry in enemyPrefabs)
        {
            enemyPrefabsDict[entry.type] = entry.prefab;
        }
    }

    void Update()
    {
        if (Time.time - timer >= spawnInterval){
            Create(transform.position, EnemyType.SmallEnemy).WithPatrolCenter(transform.position).Build();
            timer = Time.time;
        }
    }

    public EnemyBuilder Create(Vector3 spawnPoint, EnemyType type)
    {
        if (!enemyPrefabsDict.TryGetValue(type, out GameObject prefab)){
            Debug.LogError("No Prefab Found in Dictionary");
        }
        return new EnemyBuilder(spawnPoint, prefab);
    }

    public class EnemyBuilder
    {
        private Vector3 spawnPoint = Vector3.zero;
        private Vector3 patrolCenter = Vector3.zero;
        private GameObject prefab;

        public EnemyBuilder(Vector3 spawnPoint, GameObject prefab)
        {
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

