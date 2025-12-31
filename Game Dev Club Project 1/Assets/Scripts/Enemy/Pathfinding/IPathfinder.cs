using UnityEngine;

public interface IPathfinder
{
    TargetData targetData { get; } 
    public Vector3 CalculateNavMeshDirection(Vector3 from, Vector3 to); //navmesh
    public Vector3 CalculateInfluenceVector(Vector3 from); // local
    public void SetEnemyData(EnemyData data);
}
