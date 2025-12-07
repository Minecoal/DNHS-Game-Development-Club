using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

// Compute a simple distance-weighted influence vector based on targetsData (attractive)
// and obstaclesData (repulsive). Returns a non-normalized vector that represents
// the combined local deviation to apply on top of the NavMesh direction.
public class Pathfinder : MonoBehaviour, IPathfinder
{
    [Header("NavMesh + Local Influence")]
    [SerializeField] private float navWeight = 1.0f;
    [SerializeField] private float fieldWeight = 0.75f;
    [SerializeField] private float sampleDist = 0.5f; 

    private NavMeshSurface navMeshSurface;
    private EnemyData enemyData;

    private List<TargetData> targetsData = new List<TargetData>(); // favored (attractive)
    private List<TargetData> obstaclesData = new List<TargetData>(); // unfavored (repulsive)

    private void OnEnable()
    {
        if (PathfinderManager.Instance != null)
            PathfinderManager.Instance.RegisterPathfinder(this);
    }

    private void OnDisable()
    {
        if (PathfinderManager.Instance != null)
            PathfinderManager.Instance.UnregisterPathfinder(this);
    }

    public void InitializeTargets(IEnumerable<TargetData> targets = null, IEnumerable<TargetData> obstacles = null)
    {
        targetsData = targets != null ? new List<TargetData>(targets) : new List<TargetData>();
        obstaclesData = obstacles != null ? new List<TargetData>(obstacles) : new List<TargetData>();
    }

    public void UpdateTargets(IEnumerable<TargetData> targets, IEnumerable<TargetData> obstacles){
        targetsData = new List<TargetData>(targets);
        targetsData = new List<TargetData>(obstacles);
    }

    public void SetEnemyData(EnemyData data)
    {
        enemyData = data;

        DynamicNavMeshManager.Instance.surfaces.TryGetValue(
            enemyData.agentType, 
            out navMeshSurface
        );

        enemyData.agentTypeID = navMeshSurface.agentTypeID;
    }

    /// <summary>
    /// Calculates a direction vector based on local influences
    /// </summary>
    public Vector3 GetInfluenceVector(Vector3 sourcePosition)
    {
        Vector3 influence = Vector3.zero;
        const float eps = 0.0001f;

        // attractive points: pull toward favored positions
        foreach (var target in targetsData)
        {
            if (target == null) continue;
            Vector3 dir = target.position - sourcePosition;
            dir.y = 0f;
            float dist = dir.magnitude;
            if (dist <= eps) continue;
            Vector3 ndir = dir / dist;

            // strength falls off with distance; closer points influence more
            float strength = target.weight / (1f + dist);
            influence += ndir * strength;
        }

        // repulsive points: push away from unfavored positions
        foreach (var obs in obstaclesData)
        {
            if (obs == null) continue;
            Vector3 dir = sourcePosition - obs.position;
            dir.y = 0f;
            float dist = dir.magnitude;
            if (dist <= eps) continue;
            Vector3 ndir = dir / dist;

            // repulsion also falls off with distance
            float strength = obs.weight / (1f + dist);
            influence += ndir * strength;
        }

        return influence;
    }
    /// <summary>
    /// This method calculates the direction with respect to both the local influence vector and navmesh direction
    /// </summary>
    public Vector3 ComputeDirectionTowards(Vector3 fromPosition, Vector3 targetPosition)
    {
        Vector3 navDir = Vector3.zero;

        if (navMeshSurface != null)
        {
            Vector3 fromSample = SampleOnNavMesh(fromPosition);
            Vector3 toSample = SampleOnNavMesh(targetPosition);
            navDir = ComputeNavMeshDirection(fromSample, toSample);
        }

        Vector3 localDir = GetInfluenceVector(fromPosition).normalized;
        Vector3 blended = navDir * navWeight + localDir * fieldWeight;
        blended.y = 0f;

        return blended.sqrMagnitude < 1e-6f ? Vector3.zero : blended.normalized;
    }

    /// <summary>
    /// Find the cloest position on the Navmesh to the given position
    /// (Taking account for when the agent might be off the navmesh)
    /// </summary>
    private Vector3 SampleOnNavMesh(Vector3 pos)
    {
        if (enemyData == null) return pos;
        NavMeshHit hit;
        var surf = DynamicNavMeshManager.Instance.GetSurface(enemyData.agentType);
        int mask = 1 << surf.agentTypeID;  // <-- use agent type filter
        if (NavMesh.SamplePosition(pos, out hit, sampleDist, mask))
            return hit.position;

        return pos;
    }
    
    private Vector3 ComputeNavMeshDirection(Vector3 from, Vector3 to)
    {
        var surf = DynamicNavMeshManager.Instance.GetSurface(enemyData.agentType);
        int mask = 1 << surf.agentTypeID;  // <-- use agent type filter

        NavMeshPath path = new NavMeshPath();
        
        if (NavMesh.CalculatePath(from, to, mask, path) &&
            path.corners.Length > 1)
        {
            Vector3 dir = path.corners[1] - from;
            dir.y = 0;
            return dir.normalized;
        }

        // fallback
        Vector3 fb = to - from;
        fb.y = 0;
        return fb.normalized;
    }
}

[System.Serializable]
public class TargetData
{
    public Vector3 position;
    public float weight;

    public TargetData(Vector3 position, float weight)
    {
        this.position = position;
        this.weight = weight;
    }

    public void SetWeight(float weight)
    {
        this.weight = weight;
    }

    public void SetPos(Vector3 pos)
    {
        this.position = pos;
    }
}

