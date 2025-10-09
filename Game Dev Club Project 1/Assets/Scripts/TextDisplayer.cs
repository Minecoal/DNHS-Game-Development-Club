
using System;
using TMPro;
using UnityEngine;

public class TextDisplayer
{
    private static GameObject textContainer;
    public TextMeshPro tMPro { get; private set; }
    public GameObject textObject { get; private set; }
    private Func<string> trackedValueProvider;

    private GameObject InitTMPObject(Vector2 position, float size)
    {
        if (textContainer == null)
        {
            textContainer = new GameObject("Text Container");
        }
        GameObject textObject = new GameObject("Text Display");
        textObject.transform.SetParent(textContainer.transform);
        textObject.transform.position = position;

        tMPro = textObject.AddComponent<TextMeshPro>();
        tMPro.transform.localScale = new Vector3(size, size, size);
        tMPro.alignment = TextAlignmentOptions.Center;
        return textObject;
    }


    // Static
    public TextDisplayer(string text, Vector2 position, float size)
    {
        textObject = InitTMPObject(position, size);
        tMPro.text = text;
    }

    public TextDisplayer(string text, float x, float y, float size) : this(text, new Vector2(x, y), size) { }


    // Dynamically Updated
    public TextDisplayer(Func<string> valueProvider, Vector2 position, float size)
    {
        textObject = InitTMPObject(position, size);
        trackedValueProvider = valueProvider;
        textObject.AddComponent<TextDisplayerUpdater>().Init(this);
    }
    public TextDisplayer(Func<string> valueProvider, float x, float y, float size) : this(valueProvider, new Vector2(x, y), size) { }



    private class TextDisplayerUpdater : MonoBehaviour
    {
        private TextDisplayer displayer;
        private bool isDragging;
        private Vector3 offset;
        private static TextDisplayerUpdater selected = null;

        public void Init(TextDisplayer d) { displayer = d; }

        void Update()
        {
            displayer.UpdateTrackedText();

            // Handle click and drag
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0;
                float dist = Vector2.Distance(mouseWorld, displayer.textObject.transform.position);
                if (dist < 0.5f && selected == null) // Adjust radius for click detection
                {
                    isDragging = true;
                    selected = this;
                    offset = displayer.textObject.transform.position - mouseWorld;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                selected = null;
            }
            if (isDragging)
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0;
                displayer.textObject.transform.position = mouseWorld + offset;
            }
        }
    }

    protected void UpdateTrackedText()
    {
        if (trackedValueProvider != null)
            tMPro.text = trackedValueProvider();
    }

    public void SetUpdateTracker(Func<string> valueProvider)
    {
        trackedValueProvider = valueProvider;
    }

    public void UpdateText(string text)
    {
        tMPro.text = text;
    }

    public void UpdatePosition(Vector2 position)
    {
        textObject.transform.position = position;
    }

    public void UpdatePosition(float x, float y)
    {
        UpdatePosition(new Vector2(x, y));
    }

    public void UpdateTextAndPosition(string text, Vector2 position)
    {
        UpdateText(text);
        UpdatePosition(position);
    }

    public void UpdateTextAndPosition(string text, float x, float y)
    {
        UpdateText(text);
        UpdatePosition(x, y);
    }
}
