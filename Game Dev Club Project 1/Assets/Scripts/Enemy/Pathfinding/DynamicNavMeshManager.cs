using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;


public class DynamicNavMeshManager : MonoBehaviour
{
    public static DynamicNavMeshManager Instance;
    public Dictionary<AgentType, NavMeshSurface> surfaces;
    private Dictionary<AgentType, NavMeshDataInstance> dataInstances;

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

        surfaces = new Dictionary<AgentType, NavMeshSurface>();
        dataInstances = new Dictionary<AgentType, NavMeshDataInstance>();
    }

    public void RegisterSurface(AgentType agentType, NavMeshSurface surface){
        surfaces.Add(agentType, surface);
        NavMeshData data = surface.navMeshData;
        NavMeshDataInstance instance = NavMesh.AddNavMeshData(data);
        dataInstances.Add(agentType, instance);
    }

    public void UnregisterSurface(AgentType agentType){
        if (dataInstances.TryGetValue(agentType, out NavMeshDataInstance instance)){
            NavMesh.RemoveNavMeshData(instance);
            dataInstances.Remove(agentType);
            surfaces.Remove(agentType);
        } else {
            Debug.LogWarning("Unregistration of NavMesh Surface Failed");
        }
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
