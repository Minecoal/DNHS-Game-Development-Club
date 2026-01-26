using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float detectionRadius;
    public float chaseRadius;
    public float attackRadius;

    public float health;
    public float damage;
    public float speed;
    public float accelAmount;
    public float decelAmount;

    public AgentType agentType;
    public int agentTypeID;

    [Header("Pathfinding Related")]
    public float vectorFieldRadius;
    public float weight;

    [Header("Drop Table")]
    public DropTableClass[] dropTable;
}
