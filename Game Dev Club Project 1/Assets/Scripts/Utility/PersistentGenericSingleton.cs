using System;
using UnityEngine;

public class PersistentGenericSingleton<T> : MonoBehaviour where T : Component
{
    public bool autoUnparentOnAwake = true;
    
    public bool IsReady { get; private set; } = false;
    public Action OnReady;

    protected static T instance;
    public static T Instance {
        get {
            if (instance == null) {
                instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    if (!Application.isPlaying) return null; // don't auto-create when closing
                    GameObject obj = new GameObject(typeof(T).Name + " Auto Generated" );
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    protected virtual void Awake() {
        InitializeSingleton();
        IsReady = true;
        OnReady?.Invoke();
    } 

    protected virtual void InitializeSingleton(){
        if (!Application.isPlaying) return;
        if (autoUnparentOnAwake)
            transform.SetParent(null);

        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        } else { 
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}