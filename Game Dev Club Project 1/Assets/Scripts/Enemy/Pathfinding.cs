using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Compute a simple distance-weighted influence vector based on targetsData (attractive)
// and obstaclesData (repulsive). Returns a non-normalized vector that represents
// the combined local deviation to apply on top of the NavMesh direction.
public class Pathfinding : MonoBehaviour, IPathfinder
{
    [Header("Target Settings")]
    // Per-target weights are stored on TargetData; global fallbacks removed to avoid confusion.

    [Header("NavMesh + Local Influence")]
    [Tooltip("Weight of the NavMesh global direction when combining with local influence")]
    [SerializeField] private float navWeight = 1.0f;
    [Tooltip("Weight of the local influence (attraction/repulsion) relative to NavMesh")]
    [SerializeField] private float fieldWeight = 0.75f;
    [Tooltip("Local variance magnitude applied to NavMesh waypoint using the influence vector")]
    [SerializeField] private float variance = 0.5f;

    private List<TargetData> targetsData = new List<TargetData>(); // favored (attractive)
    private List<TargetData> obstaclesData = new List<TargetData>(); // unfavored (repulsive)
    private Vector3 destinationTarget;
    private bool hasDestination = false;

    // Agent settings (injected from EnemyData)
    private float agentRadius = 0.5f;
    private float agentHeight = 2f;
    private float agentStepHeight = 0.4f;
    private float agentSlopeLimit = 45f;
    private float agentSpeed = 3.5f;


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

    public void Initialize(IEnumerable<TargetData> targets = null, IEnumerable<TargetData> obstacles = null)
    {
        targetsData = targets != null ? new List<TargetData>(targets) : new List<TargetData>();
        obstaclesData = obstacles != null ? new List<TargetData>(obstacles) : new List<TargetData>();
    }

    // Inject agent/nav settings from an EnemyData instance so pathfinding
    // can sample/compute against the NavMesh with appropriate agent tolerances.
    public void SetAgentSettings(EnemyData data)
    {
        if (data == null) return;
        agentRadius = data.agentRadius;
        agentHeight = data.agentHeight;
        agentStepHeight = data.agentStepHeight;
        agentSlopeLimit = data.agentSlopeLimit;
        agentSpeed = data.agentSpeed;
    }

    // Set the player/world target position. This is the single true destination
    // the NavMesh path will be computed towards.
    public void SetDestinationTarget(Vector3 destinationPos)
    {
        destinationTarget = destinationPos;
        hasDestination = true;
    }

    public Vector3 ComputeDirectionTowards(Vector3 fromPosition, Vector3 targetPosition)
    {
        // Use NavMesh to get a coarse global direction towards the explicit targetPosition.
        // First, sample positions on the NavMesh near the source and target using the agent radius,
        // so the computed path is biased toward valid positions for this agent size.
        Vector3 navDir = Vector3.zero;
        NavMeshPath path = new NavMeshPath();
        Vector3 fromSample = fromPosition;
        Vector3 toSample = targetPosition;
        NavMeshHit hit;
        float sampleDist = Mathf.Max(agentRadius * 2f, 0.5f);
        if (NavMesh.SamplePosition(fromPosition, out hit, sampleDist, NavMesh.AllAreas))
            fromSample = hit.position;
        if (NavMesh.SamplePosition(targetPosition, out hit, sampleDist, NavMesh.AllAreas))
            toSample = hit.position;

        if (NavMesh.CalculatePath(fromSample, toSample, NavMesh.AllAreas, path) && path.corners != null && path.corners.Length > 1)
        {
            Vector3 nextCorner = path.corners[1];
            navDir = nextCorner - fromSample;
            navDir.y = 0f;
            if (navDir.sqrMagnitude > 1e-6f) navDir = navDir.normalized;
            else navDir = Vector3.zero;
        }
        else
        {
            // fallback to direct vector
            navDir = targetPosition - fromPosition;
            navDir.y = 0f;
            if (navDir.sqrMagnitude > 1e-6f) navDir = navDir.normalized;
            else navDir = Vector3.zero;
        }

        Vector3 localField = GetInfluenceVector(fromPosition).normalized;

        // Blend directions: NavMesh global direction + local influence (attraction/repulsion)
        Vector3 blended = navDir * navWeight + localField * fieldWeight;
        blended.y = 0f;
        if (blended.sqrMagnitude < 1e-6f) return Vector3.zero;
        return blended.normalized;
    }

    public Vector3 ComputeBestTargetPosition(Vector3 fromPosition)
    {
        // If we have an externally injected a destination, use NavMesh towards it
        if (hasDestination)
        {
            Vector3 navTarget = destinationTarget;
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(fromPosition, destinationTarget, NavMesh.AllAreas, path) && path.corners != null && path.corners.Length > 1)
            {
                // prefer the next corner as the coarse target
                navTarget = path.corners[1];
            }

            // local vector field adds variance and local avoidance behavior
            var localDir = GetInfluenceVector(fromPosition).normalized;
            Vector3 finalTarget = navTarget + localDir * variance;
            return finalTarget;
        }

        // no destination: fall back to local vector field only
        var fallbackDir = GetInfluenceVector(fromPosition).normalized;
        return fromPosition + fallbackDir * variance;
    }

    public void SetUpdateInterval(float seconds)
    {
        //TODO: schedule internal updates, not implemented yet. Potentially managed by pathfinder manager
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

