using UnityEngine;
using System;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }
    public Transform[] patrolPoints;
    public float detectionRadius = 5f;
    public float attackRadius = 1f;

    private Health health;
    private Transform player;

    public event Action<DamageInfo> OnDamagedBy;

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        health = GetComponent<Health>();
        health.OnDied += HandleDeath;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        StateMachine.Tick(this, Time.deltaTime);
    }

    private void HandleDeath()
    {
        // Default death behavior
        gameObject.SetActive(false);
    }

    public bool IsPlayerInDetectionRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= detectionRadius;
    }

    public bool IsPlayerInAttackRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= attackRadius;
    }

    public Transform GetPlayerTransform() => player;
}
