using UnityEngine;

public class NavMeshUpdater : MonoBehaviour
{
    private NavMeshSurfaceBinding[] bindings;

    void Awake()
    {
        bindings = GetComponents<NavMeshSurfaceBinding>();
    }
    
    private void OnEnable()
    {
        DynamicNavMeshManager instance = DynamicNavMeshManager.Instance; // this is garanteed -- go see GenericSingleton

        if (instance.IsReady)
        {
            RegisterToInstance();
        }
        else
        {
            // Wait for OnReady event
            instance.OnReady += RegisterToInstance;
        }
    }

    private void OnDisable()
    {
        DynamicNavMeshManager instance = DynamicNavMeshManager.Instance;
        if (instance == null) return;

        instance.OnReady -= RegisterToInstance; // unsubscribe in case we never registered
        if (bindings == null) return;
        foreach (var binding in bindings) {
            DynamicNavMeshManager.Instance.UnregisterSurface(binding.agentType);
            Debug.Log("Unregisterd Surface");
        }
    }

    private void RegisterToInstance()
    {
        if (bindings == null) return;
        foreach (var binding in bindings)
        {
            DynamicNavMeshManager.Instance.RegisterSurface(binding.agentType, binding.surface);
            Debug.Log("Registerd Surface");
        }
    }
}
