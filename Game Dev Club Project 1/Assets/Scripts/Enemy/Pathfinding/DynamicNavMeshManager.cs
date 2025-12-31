using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class DynamicNavMeshManager : PersistentGenericSingleton<DynamicNavMeshManager>
{
    public Dictionary<AgentType, NavMeshSurface> surfaces;
    private Dictionary<AgentType, NavMeshDataInstance> dataInstances;

    protected override void Awake()
    {
        base.Awake();
        surfaces = new Dictionary<AgentType, NavMeshSurface>();
        dataInstances = new Dictionary<AgentType, NavMeshDataInstance>();
    }

    public void RegisterSurface(AgentType agentType, NavMeshSurface surface)
    {
        if (surfaces.ContainsKey(agentType))
            Debug.LogWarning("An Agent of the same type has already been registered");

        if (surface.navMeshData == null)
            surface.BuildNavMesh();
        surfaces.Add(agentType, surface);

        NavMeshDataInstance instance = NavMesh.AddNavMeshData(surface.navMeshData);
        dataInstances.Add(agentType, instance);
    }

    public void UnregisterSurface(AgentType agentType){

        if (!dataInstances.TryGetValue(agentType, out NavMeshDataInstance instance)){
            Debug.LogWarning("Unregistration of NavMesh Surface Failed");
            return;
        }

        NavMesh.RemoveNavMeshData(instance); // this is the entire reason for dataInstances
        dataInstances.Remove(agentType);
        surfaces.Remove(agentType);
    }

    public NavMeshSurface GetSurface(AgentType type)
    {
        return surfaces[type];
    }

    public void Rebuild(AgentType type)
    {
        surfaces[type].BuildNavMesh();
    }

    public void RebuildAll()
    {
        foreach (var surface in surfaces.Values)
            surface.BuildNavMesh();
    }
}

[Serializable]
public enum AgentType
{
    Small,
    Big
}
