using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Combat/Hitbox Registry"), Serializable]
public class HitboxRegistry : ScriptableObject
{
    public HitboxPrefabEntry[] entries;

    // Runtime dictionary for fast lookup
    private Dictionary<HitboxType, GameObject> prefabEntryDict;
    private Dictionary<HitboxType, HitboxData> dataEntryDict;

    public void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        prefabEntryDict = new Dictionary<HitboxType, GameObject>();
        dataEntryDict = new Dictionary<HitboxType, HitboxData>();

        if (entries == null) return;
        foreach (var e in entries)
        {
            if (!e.prefab) Debug.LogWarning($"Missing prefab for {e.type}");
            prefabEntryDict[e.type] = e.prefab;

            if (!e.hitboxData) Debug.LogWarning($"Missing Hitbox Data for {e.type}");
            dataEntryDict[e.type] = e.hitboxData;
        }
    }

    public GameObject GetPrefab(HitboxType type)
    {
        if (!prefabEntryDict.TryGetValue(type, out GameObject prefab)) Debug.LogWarning("No Hitbox Prefab found in dict");
        return prefab;
    }

    public HitboxData GetData(HitboxType type)
    {
        if (!dataEntryDict.TryGetValue(type, out HitboxData data)) Debug.LogWarning("No Hitbox Data found in dict");
        return data;
    }
}

[Serializable]
public struct HitboxPrefabEntry
{
    public HitboxType type;
    public GameObject prefab;
    public HitboxData hitboxData;
}

[Serializable]
public enum HitboxType
{
    SmallHorizontal,
    LargeVertical
}
