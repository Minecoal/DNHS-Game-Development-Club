using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all IPathfinder instances in the scene
/// Provides centralized storage and distribution of NavMesh surfaces and EnemyData
/// </summary>
public class PathfinderManager : PersistentGenericSingleton<PathfinderManager>
{
    public readonly List<TargetData> globalTargetData = new List<TargetData>();
    public readonly List<TargetData> globalObstacleData = new List<TargetData>();
    public readonly List<IPathfinder> globalPathfinders = new List<IPathfinder>();

    public void RegisterPathfinder(IPathfinder pathfinder)
    {
        if (pathfinder == null) return;
        globalPathfinders.Add(pathfinder);
    }

    public void UnregisterPathfinder(IPathfinder pathfinder)
    {
        if (pathfinder == null) return;
        globalPathfinders.Remove(pathfinder);
    }

    public void RegisterTarget(TargetData target)
    {
        if (target == null) return;
        globalTargetData.Add(target);
    }
    public void UnregisterTarget(TargetData target)
    {
        if (target == null) return;
        globalTargetData.Remove(target);
    }

    public void RegisterObstacle(TargetData target)
    {
        if (target == null) return;
        globalObstacleData.Add(target);
    }
    public void UnregisterObstacle(TargetData target)
    {
        if (target == null) return;
        globalObstacleData.Remove(target);
    }
}
