using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// Manages all IPathfinder instances in the scene
/// Provides centralized storage and distribution of NavMesh surfaces and EnemyData
/// </summary>
public class PathfinderManager : MonoBehaviour
{
    public static PathfinderManager Instance;

    private HashSet<IPathfinder> pathfinders = new HashSet<IPathfinder>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Disable when parented to a DonDestoryOnLoad object
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RegisterPathfinder(IPathfinder pathfinder)
    {
        if (pathfinder == null) return;
        pathfinders.Add(pathfinder);
    }

    public void UnregisterPathfinder(IPathfinder pathfinder)
    {
        if (pathfinder == null) return;
        pathfinders.Remove(pathfinder);
    }

    public IEnumerable<IPathfinder> GetAllPathfinders()
    {
        return new List<IPathfinder>(pathfinders);
    }
}
