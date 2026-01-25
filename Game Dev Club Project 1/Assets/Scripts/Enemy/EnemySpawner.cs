using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] static private Dictionary<EnemyType, GameObject> enemyPrefabsDict;

    public static EnemyBuilder New(Vector3 spawnPoint, EnemyType type)
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
            GameObject obj = Instantiate(prefab, spawnPoint, Quaternion.identity);
            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.SetPatrolCenter(patrolCenter);
            enemy.Initialize();
            return obj;
        }
    }
}

