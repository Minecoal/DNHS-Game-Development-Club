using UnityEngine;
using System;
using UnityEngine.UI;

//Call TextDisplayManager.New(...) from main thread.
public class TextDisplayManager : PersistentGenericSingleton<TextDisplayManager>
{
    [SerializeField] private Canvas canvas;
    private GameObject container;
    [SerializeField] private Font defaultFont;

    protected override void Awake()
    {
        base.Awake();
        if (defaultFont == null)
        {
            defaultFont = Resources.Load<Font>("Fonts/Dropline");
        }
    }

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

        // Use normal Transform for 3D text
        if (parent != null) {
            textObject.transform.localPosition = position;
            textObject.transform.SetParent(parent, false); // don't keep world position
        } else {
            textObject.transform.SetParent(containerObj.transform, true); // keep world position
            textObject.transform.position = position;
        }

        TextMesh tm = textObject.AddComponent<TextMesh>();
        if (defaultFont != null) tm.font = defaultFont;
        tm.anchor = TextAnchor.MiddleCenter;
        // fontSize and characterSize together control rendered size; pick sensible defaults
        tm.fontSize = Mathf.Max(1, Mathf.RoundToInt(40 * Mathf.Max(1f, size)));
        tm.characterSize = Mathf.Max(0.1f, size * 0.1f);
        tm.color = Color.white;
        if (!string.IsNullOrEmpty(initialText)) tm.text = initialText;

        // ensure mesh renderer uses the font material so glyphs render
        var mr = textObject.GetComponent<MeshRenderer>();
        if (mr != null && defaultFont != null && defaultFont.material != null)
        {
            mr.sharedMaterial = defaultFont.material;
        }

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

    ///<summary>Create a UI Text (UnityEngine.UI.Text) under a Canvas. anchoredPosition is in canvas local coordinates (RectTransform.anchoredPosition).</summary>
    public TextDisplay CreateUI(Vector2 anchoredPosition, float size, string initialText = null, Func<string> trackedProvider = null, Canvas parentCanvas = null, Action onClick = null, bool draggable = false, float autoDestroySeconds = 0f)
    {
        // find or create canvas
        Canvas canvas = this.canvas;
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("TextDisplay Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            DontDestroyOnLoad(canvasGO);
            this.canvas = canvas;
        }

        GameObject textObject = new GameObject("Text UI");
        textObject.transform.SetParent(canvas.transform, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0.5f); // center pivot
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(200f, 50f);
        rect.localScale = Vector3.one;

        canvas.sortingOrder = 100; // make sure it renders on top

        // Ensure CanvasRenderer present for UI rendering
        if (textObject.GetComponent<CanvasRenderer>() == null)
            textObject.AddComponent<CanvasRenderer>();

        var tm = textObject.AddComponent<Text>();
        if (defaultFont != null) tm.font = defaultFont;
        tm.color = Color.white;
        tm.alignment = TextAnchor.MiddleCenter;
        tm.fontSize = Mathf.Max(1, Mathf.RoundToInt(size * 50f));
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