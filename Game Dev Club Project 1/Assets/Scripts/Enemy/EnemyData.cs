using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float detectionRadius;
    public float chaseRadius;
    public float attackRadius;

    public float health;
    public float damage;
    // NavMesh/agent tuning
    [Header("Agent Settings")]
    public float agentRadius = 0.5f;
    public float agentHeight = 2f;
    public float agentStepHeight = 0.4f;
    public float agentSlopeLimit = 45f; // degrees
    public float agentSpeed = 3.5f;
}
