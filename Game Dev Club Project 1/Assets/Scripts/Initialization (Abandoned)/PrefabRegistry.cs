using UnityEngine;

[CreateAssetMenu(fileName = "PrefabRegistry", menuName = "Prefab Registry")]
public class PrefabRegistry : ScriptableObject
{
    [Tooltip("All prefabs that should be spawned on game initialization")]
    public GameObject[] prefabs = new GameObject[0];
}
