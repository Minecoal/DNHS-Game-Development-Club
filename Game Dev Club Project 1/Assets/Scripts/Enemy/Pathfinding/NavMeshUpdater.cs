using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshUpdater : MonoBehaviour
{
    private AgentType[] agentTypes;
    private NavMeshSurface[] surfaces;

    void Awake()
    {
        surfaces = GetComponents<NavMeshSurface>();
        agentTypes = (AgentType[])Enum.GetValues(typeof(AgentType)); // gets the enums in an array
        
        if (agentTypes != null && surfaces != null)
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                DynamicNavMeshManager.Instance.RegisterSurface(agentTypes[i], surfaces[i]);
                Debug.Log("Registerd Surface");
            }
        }
    }
}
