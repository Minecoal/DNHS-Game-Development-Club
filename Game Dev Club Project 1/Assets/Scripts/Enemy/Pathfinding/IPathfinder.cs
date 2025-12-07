using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public interface IPathfinder
{
    void InitializeTargets(IEnumerable<TargetData> targets = null, IEnumerable<TargetData> obstacles = null);
    void UpdateTargets(IEnumerable<TargetData> targets, IEnumerable<TargetData> obstacles);
    Vector3 ComputeDirectionTowards(Vector3 fromPosition, Vector3 targetPosition);
    public void SetEnemyData(EnemyData data);
}
