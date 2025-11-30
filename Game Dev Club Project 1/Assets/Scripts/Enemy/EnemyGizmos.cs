using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Enemy))]
public class EnemyGizmos : MonoBehaviour
{
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private bool showState = true;
    [SerializeField] private int segments = 32;

    private Enemy enemy;

    // shared mesh cache to avoid allocations across instances
    private static readonly Dictionary<string, Mesh> meshCache = new Dictionary<string, Mesh>();

    private void Reset()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnValidate()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        if (enemy == null) enemy = GetComponent<Enemy>();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(enemy.transform.position, enemy.chaseRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemy.transform.position, enemy.detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemy.transform.position, enemy.attackRadius);
    }

    void Start()
    {
        if (showState){
            TextDisplayManager.New3D(new Vector3(0, 2f, 0), 0.25f).WithParent(transform).WithTrackedProvider(()=>enemy.GetState()).Build();
        }
    }
}
