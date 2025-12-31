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
    [SerializeField] private float sampleDist = 0.5f; 

    private NavMeshSurface _navMeshSurface;
    private NavMeshPath _navPath;
    private EnemyData _enemyData;
    public TargetData targetData { get; private set; }

    private List<TargetData> _targets;
    private List<TargetData> _obstacles;
    
    private void Awake()
    {
        targetData = new TargetData();
        _navPath = new NavMeshPath();
    }

    private void OnEnable()
    {
        PathfinderManager instance = PathfinderManager.Instance; // this is garanteed -- go see GenericSingleton

        if (instance.IsReady)
        {
            RegisterToInstance();
        }
        else
        {
            // Wait for OnReady event
            instance.OnReady += RegisterToInstance;
        }
    }

    private void OnDisable()
    {
        var instance = PathfinderManager.Instance;
        if (instance == null) return;

        instance.OnReady -= RegisterToInstance; // unsubscribe in case we never registered
        instance.UnregisterObstacle(targetData);
        instance.UnregisterPathfinder(this);
    }

    private void RegisterToInstance()
    {
        PathfinderManager instance = PathfinderManager.Instance;
        instance.RegisterPathfinder(this);
        instance.RegisterObstacle(targetData);

        _targets = instance.globalTargetData;
        _obstacles = instance.globalObstacleData;
    }

    public void SetEnemyData(EnemyData data)
    {
        _enemyData = data;

        DynamicNavMeshManager.Instance.surfaces.TryGetValue(
            _enemyData.agentType,
            out _navMeshSurface
        );

        _enemyData.agentTypeID = _navMeshSurface.agentTypeID;
        targetData.weight = _enemyData.weight;
    }

    private void FixedUpdate()
    {
        targetData.position = transform.position;
    }

    public static float DistanceFalloff(float distance, float strength, float falloff) // graph it on desmos lol
    {
        return strength / (1f + falloff * distance);
    }
    /// <summary>
    /// Calculates a direction vector based on local influences
    /// </summary>
    public Vector3 CalculateInfluenceVector(Vector3 sourcePosition)
    {
        int strength = 8;
        int falloff = 80;

        Vector3 influence = Vector3.zero;
        if (_enemyData == null) return Vector3.zero;
        float radius = _enemyData.vectorFieldRadius;

        if (_targets == null || _obstacles == null)
        {
            Debug.LogError("Pathfinder not registered correctly.");
            return Vector3.zero;
        }

        // attractive points: pull toward favored positions
        foreach (var target in _targets)
        {
            if (target == null) continue;
            Vector3 dir = target.position - sourcePosition;
            dir.y = 0f;
            float dist = dir.magnitude;
            if (dist > radius) continue; // only check for targets within a certain radius

            influence += dir.normalized * DistanceFalloff(dist, strength, falloff) * target.weight;
        }

        // repulsive points: push away from unfavored positions
        foreach (var obs in _obstacles)
        {
            if (obs == null) continue;
            Vector3 dir = sourcePosition - obs.position;
            dir.y = 0f;
            float dist = dir.magnitude;
            if (dist > radius) continue; // only check for targets within a certain radius

            influence += dir.normalized * DistanceFalloff(dist, strength, falloff) * obs.weight;
        }

        return influence;
    }

    public Vector3 CalculateNavMeshDirection(Vector3 from, Vector3 to)
    {
        if (_navMeshSurface == null)
            return (to - from).normalized;

        NavMeshQueryFilter filter = new NavMeshQueryFilter();
        filter.agentTypeID = _enemyData.agentTypeID;
        filter.areaMask = NavMesh.AllAreas;

        Vector3 fromSample = SampleOnNavMesh(from, filter);
        Vector3 toSample = SampleOnNavMesh(to, filter);
        
        if (NavMesh.CalculatePath(fromSample, toSample, filter, _navPath) && _navPath.corners.Length > 1)
        {
            Vector3 dir = _navPath.corners[1] - fromSample;
            dir.y = 0;
            return dir.normalized;
        }

        // fallback
        Vector3 fb = toSample - fromSample;
        fb.y = 0;
        return fb.normalized;
    }

    /// <summary>
    /// Find the cloest position on the Navmesh to the given position
    /// (Taking account for when the agent might be off the navmesh)
    /// </summary>
    private Vector3 SampleOnNavMesh(Vector3 pos, NavMeshQueryFilter filter)
    {
        if (_enemyData == null) return pos;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(pos, out hit, sampleDist, filter.areaMask))
            return hit.position;

        return pos;
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

    public TargetData() : this(Vector3.zero, 0f) {}
}

