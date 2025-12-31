using System;
using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T : Component
{
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
        if (instance == null) {
            instance = this as T;
        } else { 
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
