using UnityEngine;
using UnityEngine.AI;

public interface IPathfinder
{
    TargetData targetData { get; } 
    public Vector3 CalculateNavMeshDirection(Vector3 from, Vector3 to); //navmesh
    public Vector3 CalculateInfluenceVector(Vector3 from); // local
    public Vector3 SampleOnNavMesh(Vector3 pos); // finder nearest point on navmesh
    public void SetEnemyData(EnemyData data);
}
