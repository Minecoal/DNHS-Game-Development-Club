using UnityEngine;
using System;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Pathfinder))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }

    public Rigidbody Rb { get; private set; }
    public Health Health { get; private set; }
    public IPathfinder Pathfinder { get; private set; }
    private Transform playerTransform;
    [SerializeField] private EnemyData enemyData;
    public EnemyData EnemyData => enemyData;
    public Vector3 patrolCenter { get; private set; } = Vector3.zero;

    public event Action<DamageInfo> OnDamagedBy;
    public EnemyContext context { get; private set; }

    private bool isInitialized = false;

    public void SetPatrolCenter(Vector3 patrolCenter)
    {
        this.patrolCenter = patrolCenter;
    }

    public void Initialize()
    {
        StateMachine = new EnemyStateMachine();
        Rb = GetComponent<Rigidbody>();
        Health = GetComponent<Health>();
        playerTransform = PlayerManager.Instance.Player.transform;
        Pathfinder = GetComponent<Pathfinder>();
        Health.OnDied += HandleDeath;
        
        context = new EnemyContext(
            StateMachine,
            this,
            Rb,
            Health,
            playerTransform,
            Pathfinder,
            enemyData,
            patrolCenter
        );

        if (enemyData != null)
            Pathfinder.SetEnemyData(enemyData);
        
        Collider collider = GetComponent<BoxCollider>();
        collider.enabled = false;

        IEnemyState initial = (IEnemyState)new EnemyIdleState();
        StateMachine.Initialize(initial, context);

        isInitialized = true;
    }

    private void AutoInit() // for when enemy prefab is manually placed into the scene
    {
        patrolCenter = transform.position;
        Initialize();
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) AutoInit();
        StateMachine.Tick(context, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!isInitialized) AutoInit();
        StateMachine.FixedTick(context, Time.fixedDeltaTime);

        Decel(context.EnemyData.decelAmount);
        UpdateVectorField(transform.position);
    }

    public void MoveTowardsPosition(Vector3 currentPos, Vector3 targetPos, float moveSpeed, float accelAmount, float decelAmount)
    {
        // Compute direction towards target
        Vector3 dir = Pathfinder.CalculateNavMeshDirection(currentPos, targetPos);
        dir.Normalize();

        // Desired target velocity
        Vector3 targetVelocity = dir * moveSpeed;

        // Acceleration/deceleration per axis
        Vector3 accelRate = new Vector3(
            Mathf.Abs(targetVelocity.x) > 0.01f ? accelAmount : decelAmount,
            0f,
            Mathf.Abs(targetVelocity.z) > 0.01f ? accelAmount : decelAmount
        );

        // Conserve momentum: don't decelerate if we are already faster than target
        if (Mathf.Abs(Rb.linearVelocity.x) > Mathf.Abs(targetVelocity.x) && Mathf.Sign(Rb.linearVelocity.x) == Mathf.Sign(targetVelocity.x))
            accelRate.x = 0;
        if (Mathf.Abs(Rb.linearVelocity.z) > Mathf.Abs(targetVelocity.z) && Mathf.Sign(Rb.linearVelocity.z) == Mathf.Sign(targetVelocity.z))
            accelRate.z = 0;

        // Difference between current velocity and target velocity
        Vector3 velocityDiff = targetVelocity - new Vector3(Rb.linearVelocity.x, 0f, Rb.linearVelocity.z);

        // Calculate force to apply
        Vector3 force = new Vector3(
            velocityDiff.x * accelRate.x,
            0f,
            velocityDiff.z * accelRate.z
        );

        // Apply forces along world axes
        Rb.AddForce(force.x * Vector3.right, ForceMode.Force);
        Rb.AddForce(force.z * Vector3.forward, ForceMode.Force);

        // Smooth rotation toward movement direction
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            Rb.rotation = Quaternion.Slerp(Rb.rotation, targetRot, 10f * Time.fixedDeltaTime);
        }
    }

    public void MoveTowardsPosition(Vector3 targetPos, float speed, float accelAmount, float decelAmount)
    {
        MoveTowardsPosition(transform.position, targetPos, speed, accelAmount, decelAmount);
    }

    public void UpdateVectorField(Vector3 currentPos)
    {
        Vector3 force = Pathfinder.CalculateInfluenceVector(currentPos);

        Rb.AddForce(force.x * Vector3.right, ForceMode.Force);
        Rb.AddForce(force.z * Vector3.forward, ForceMode.Force);
        
    }

    public void Decel(float decelAmount)
    {
        Vector3 movement = new Vector3(-Rb.linearVelocity.x * decelAmount, 0f, -Rb.linearVelocity.z * decelAmount);
        Rb.AddForce(movement.x * Vector3.right, ForceMode.Force);
        Rb.AddForce(movement.y * Vector3.up, ForceMode.Force);
        Rb.AddForce(movement.z * Vector3.forward, ForceMode.Force);
    }

    private void HandleDeath()
    {
        gameObject.SetActive(false); //temporary
    }

    public bool IsPlayerInDetectionRange()
    {
        if (playerTransform == null) return false;
        return Vector3.Distance(transform.position, playerTransform.position) <= enemyData.detectionRadius;
    }

    public bool IsPlayerInAttackRange()
    {
        if (playerTransform == null) return false;
        return Vector3.Distance(transform.position, playerTransform.position) <= enemyData.attackRadius;
    
    }
    public bool IsPlayerInChaseRange()
    {
        if (playerTransform == null) return false;
        return Vector3.Distance(transform.position, playerTransform.position) <= enemyData.chaseRadius;
    }

    public Transform GetPlayerTransform() => playerTransform;

    public string GetStateName()
    {
        return StateMachine.GetStateName();     
    }
}