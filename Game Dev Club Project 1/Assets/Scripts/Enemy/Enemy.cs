using UnityEngine;
using System;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }
    public Transform[] patrolPoints;
    [SerializeField] private EnemyData enemyData;

    private Health health;
    private Transform player;

    public event Action<DamageInfo> OnDamagedBy;
    public IPathfinder Pathfinder { get; private set; }

    

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        health = GetComponent<Health>();
        health.OnDied += HandleDeath;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // wire up the Pathfinding implementation
        var pf = GetComponent<Pathfinding>();
        if (pf != null) Pathfinder = pf;
    }

    private void Start()
    {
        if (Pathfinder != null)
        {
            Pathfinder.SetDestinationTarget(transform.position); //don't target anything
            var targetList = new System.Collections.Generic.List<TargetData>(); //temp
            var obstacleList = new System.Collections.Generic.List<TargetData>(); //temp supplied by pathfinder manager later
            Pathfinder.Initialize(targetList, obstacleList);
            // Inject agent settings from EnemyData (if provided)
            if (enemyData != null)
                Pathfinder.SetAgentSettings(enemyData);
        }

        IEnemyState initial = (patrolPoints != null && patrolPoints.Length > 0) ? (IEnemyState)new EnemyPatrolState() : new EnemyIdleState();
        StateMachine.Initialize(initial, this);
    }

    //temporary movement function
    public void MoveTowardsPosition(Vector3 worldPos, float speed, float deltaTime)
    {
        Vector3 target = new Vector3(worldPos.x, transform.position.y, worldPos.z);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * deltaTime);
        Vector3 dir = (target - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * deltaTime);
        }
    }

    private void Update()
    {
        StateMachine.Tick(this, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        StateMachine.FixedTick(this, Time.fixedDeltaTime);
    }

    private void HandleDeath()
    {
        gameObject.SetActive(false); //temporary
    }

    public bool IsPlayerInDetectionRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= enemyData.detectionRadius;
    }

    public bool IsPlayerInAttackRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= enemyData.attackRadius;
    
    }
    public bool IsPlayerInChaseRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= enemyData.chaseRadius;
    }

    public Transform GetPlayerTransform() => player;

    public string GetState()
    {
   return StateMachine.GetState();     
    }
}
