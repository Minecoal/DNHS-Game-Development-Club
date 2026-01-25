using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

[CustomEditor(typeof(AnimationList))]
public class AnimationListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AnimationList list = (AnimationList)target;

        if (GUILayout.Button("Populate from Animator"))
        {
            PopulateFromAnimator(list);
        }

        if (GUILayout.Button("Compute Hashes"))
        {
            ComputeHashes(list);
        }      

        if (GUILayout.Button("Sync AnimationID Names"))
        {
            SyncAnimationIDNames(list);
        }      
    }

    private void ComputeHashes(AnimationList list)
    {
        if (list.entries == null) return;

        for (int i = 0; i < list.entries.Length; i++)
        {
            list.entries[i].hash = Animator.StringToHash(list.entries[i].name);
        }

        EditorUtility.SetDirty(list);
        AssetDatabase.SaveAssets();
    }

    private void PopulateFromAnimator(AnimationList list)
    {
        if (list.controllerReference == null)
        {
            Debug.LogWarning("Animation Controller reference not set in AnimationList.");
            return;
        }
        AnimatorController controller = list.controllerReference as AnimatorController;


        AnimationEntry[] oldEntries = list.entries;
        List<AnimationEntry> newEntries = new List<AnimationEntry>();
        
        int i = 0;
        foreach (var layer in controller.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                AnimationClip clip = state.state.motion as AnimationClip;
                newEntries.Add(new AnimationEntry
                {
                    id = oldEntries[i].id,
                    name = state.state.name, // gets the editor state name rather than clip name, this took so long :(
                    hash = Animator.StringToHash(state.state.name)
                });
                i++;
            }
        }

        list.entries = newEntries.ToArray();

        EditorUtility.SetDirty(list);
        AssetDatabase.SaveAssets();
    }

    private void SyncAnimationIDNames(AnimationList list)
    {
        if (list.entries == null) return;

        bool dirty = false;

        for (int i = 0; i < list.entries.Length; i++)
        {
            var entry = list.entries[i];
            if (entry.id != null)
            {
                if (entry.id.animationName != entry.name)
                {
                    entry.id.animationName = entry.name;
                    EditorUtility.SetDirty(entry.id); // mark the AnimationID asset dirty
                    dirty = true;
                }
            }
        }

        if (dirty)
            AssetDatabase.SaveAssets();

        Debug.Log("AnimationID names synced with entry names.");
    }
}