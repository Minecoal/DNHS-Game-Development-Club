using UnityEngine;
using System;
using TMPro;

//Call TextDisplayManager.New(...) from main thread.
public class TextDisplayManager : MonoBehaviour
{
    public static TextDisplayManager Instance;
    [SerializeField] private Canvas canvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Disable when parented to a DonDestoryOnLoad object
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
            container = new GameObject("3D Text Container");
            DontDestroyOnLoad(container);
        }
        return container;
    }

    ///<summary>Call TextDisplayParent.New() ... .Build() instead</summary>
    public TextDisplay Create3D(Vector3 position, float size, string initialText = null, Func<string> trackedProvider = null, Transform parent = null, System.Action onClick = null, bool draggable = false, float autoDestroySeconds = 0f)
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
            updater.SetOnClick(onClick);
        }

        if (autoDestroySeconds > 0f)
        {
            var ad = textObject.AddComponent<AutoDestroy>();
            ad.LifeSeconds = autoDestroySeconds;
        }

        return td;
    }

    ///<summary>Create a UI Text (TextMeshProUGUI) under a Canvas. anchoredPosition is in canvas local coordinates (RectTransform.anchoredPosition).</summary>
    public TextDisplay CreateUI(Vector2 anchoredPosition, float size, string initialText = null, Func<string> trackedProvider = null, Canvas parentCanvas = null, Action onClick = null, bool draggable = false, float autoDestroySeconds = 0f)
    {
        // find or create canvas
        Canvas canvas = this.canvas;
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("TextDisplay Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            DontDestroyOnLoad(canvasGO);
        }

        GameObject textObject = new GameObject("Text UI");
        textObject.transform.SetParent(canvas.transform, false);

        var rect = textObject.AddComponent<RectTransform>();
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(200f, 50f);

        var tm = textObject.AddComponent<TextMeshProUGUI>();
        tm.alignment = TextAlignmentOptions.Center;
        tm.fontSize = Mathf.Max(1f, size * 50f);
        tm.raycastTarget = true;
        if (!string.IsNullOrEmpty(initialText)) tm.text = initialText;

        TextDisplay td = new TextDisplay(textObject, tm, trackedProvider);

        draggable = false; // not draggable, may implement in the future
        if (trackedProvider != null || draggable || onClick != null)
        {
            var updater = textObject.AddComponent<TextDisplayUIUpdater>();
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
        private Vector3 position;
        private float size;
        private string initialText;
        private Func<string> trackedProvider;
        private Transform parent;
        private Action onClick;
        private bool draggable = false;
        private float autoDestroySeconds = 0f;

        private bool isUI = false;
        private Vector2 uiAnchoredPosition;
        private Canvas uiCanvas = null;

        public Builder(Vector3 position, float size)
        {
            this.position = position;
            this.size = size;
        }

        // UI builder ctor
        public Builder(Vector2 anchoredPosition, float size, Canvas parentCanvas = null)
        {
            this.isUI = true;
            this.uiAnchoredPosition = anchoredPosition;
            this.size = size;
            this.uiCanvas = parentCanvas;
        }

        public Builder WithInitialText(string text) { initialText = text; return this; }
        public Builder WithTrackedProvider(Func<string> provider) { trackedProvider = provider; return this; }
        public Builder WithParent(Transform p) { parent = p; return this; }
        public Builder WithOnClick(Action a) { onClick = a; return this; }
        public Builder WithDraggable(bool d = true) { draggable = d; return this; } // Only for UI TextDisplay
        public Builder WithAutoDestroy(float s) { autoDestroySeconds = s; return this; }

        public TextDisplay Build()
        {
            if (isUI)
            {
                return TextDisplayManager.Instance.CreateUI(uiAnchoredPosition, size, initialText, trackedProvider, uiCanvas, onClick, draggable, autoDestroySeconds);
            }
            return TextDisplayManager.Instance.Create3D(position, size, initialText, trackedProvider, parent, onClick, draggable, autoDestroySeconds);
        }
    }
    public static Builder New3D(Vector3 position, float size) => new Builder(position, size);
    public static Builder NewUI(Vector2 anchoredPosition, float size, Canvas parentCanvas = null) => new Builder(anchoredPosition, size, parentCanvas);
}