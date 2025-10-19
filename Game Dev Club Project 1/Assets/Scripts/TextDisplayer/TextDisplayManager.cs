using UnityEngine;
using System;
using TMPro;

//Call TextDisplayManager.New(...) from main thread.
public class TextDisplayManager : MonoBehaviour
{
    public static TextDisplayManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private GameObject container;

    private GameObject GetOrCreateContainer()
    {
        if (container == null)
        {
            container = new GameObject("Text Container");
            DontDestroyOnLoad(container);
        }
        return container;
    }

    ///<summary>Call TextDisplayParent.New() ... .Build() instead</summary>
    public TextDisplay Create(Vector2 position, float size, string initialText = null, Func<string> trackedProvider = null, Transform parent = null, System.Action onClick = null, bool draggable = false, float autoDestroySeconds = 0f)
    {
        GameObject containerObj = parent != null ? parent.gameObject : GetOrCreateContainer();
        GameObject textObject = new GameObject("Text Display");

        if (parent != null) {
            textObject.transform.SetParent(parent, false); // don't keep world position
        } else {
            textObject.transform.position = position;
            textObject.transform.SetParent(containerObj.transform, true); // keep world position
        }
        
        TextMeshPro tm = textObject.AddComponent<TextMeshPro>();
        tm.transform.localScale = new Vector3(size, size, size);
        tm.alignment = TextAlignmentOptions.Center;
        if (!string.IsNullOrEmpty(initialText)) tm.text = initialText;

        TextDisplay td = new TextDisplay(textObject, tm, trackedProvider);

        if (trackedProvider != null || draggable || onClick != null)
        {
            var updater = textObject.AddComponent<TextDisplayUpdater>();
            updater.Init(td, trackedProvider);
            updater.SetDraggable(draggable);
            updater.SetOnClick(onClick);
        }

        if (autoDestroySeconds > 0f)
        {
            var ad = textObject.AddComponent<AutoDestroy>();
            ad.LifeSeconds = autoDestroySeconds;
        }

        return td;
    }

    public class Builder
    {
        private Vector2 position;
        private float size;
        private string initialText;
        private Func<string> trackedProvider;
        private Transform parent;
        private Action onClick;
        private bool draggable = false;
        private float autoDestroySeconds = 0f;

        public Builder(Vector2 position, float size)
        {
            this.position = position;
            this.size = size;
        }

        public Builder WithInitialText(string text) { initialText = text; return this; }
        public Builder WithTrackedProvider(Func<string> provider) { trackedProvider = provider; return this; }
        public Builder WithParent(Transform p) { parent = p; return this; }
        public Builder WithOnClick(Action a) { onClick = a; return this; }
        public Builder WithDraggable(bool d = true) { draggable = d; return this; }
        public Builder WithAutoDestroy(float s) { autoDestroySeconds = s; return this; }

        public TextDisplay Build()
        {
            return TextDisplayManager.Instance.Create(position, size, initialText, trackedProvider, parent, onClick, draggable, autoDestroySeconds);
        }
    }

    public static Builder New(Vector2 position, float size) => new Builder(position, size);
}