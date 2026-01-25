// using UnityEngine;
// using System.Collections.Generic;
// using System;

// [CreateAssetMenu(menuName = "Combat/Attack Registry")]
// public class AttackRegistry : ScriptableObject
// {
//     public AttackEntry[] entries;

//     // Runtime dictionary for fast lookup
//     private Dictionary<AttackID, AttackData> entryDict;

//     private void OnEnable()
//     {
//         Initialize();
//     }

//     public void Initialize()
//     {
//         entryDict = new Dictionary<AttackID, AttackData>();
//         if (entries == null) return;

//         foreach (var e in entries){
//             if (!e.id || !e.data){
//                 Debug.LogWarning($"Invalid AttackEntry in {name}", this);
//                 continue;
//             }

//             entryDict[e.id] = e.data;
//         }
//     }

//     public bool TryGetData(AttackID id, out AttackData data)
//     {
//         return entryDict.TryGetValue(id, out data);
//     }

//     public AttackData GetData(AttackID id)
//     {
//         if (!entryDict.TryGetValue(id, out var data))
//             Debug.LogWarning($"AttackID '{id.name}' not found in {name}", this);

//         return data;
//     }
// }

// [Serializable]
// public struct AttackEntry
// {
//     public AttackID id;
//     public AttackData data;
// }

// [Serializable]
// public enum AttackType{
//     SmallHorizontal,
//     LargeVertical
// }
