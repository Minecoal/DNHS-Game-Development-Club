using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder
{
    void Initialize(IEnumerable<TargetData> targets = null, IEnumerable<TargetData> obstacles = null);
    Vector3 ComputeDirectionTowards(Vector3 fromPosition, Vector3 targetPosition);
    Vector3 ComputeBestTargetPosition(Vector3 fromPosition);
    void SetUpdateInterval(float seconds);
    void SetDestinationTarget(Vector3 destinationPos);
    void SetAgentSettings(EnemyData data);
}
