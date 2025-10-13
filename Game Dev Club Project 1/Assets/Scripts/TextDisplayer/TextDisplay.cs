using UnityEngine;
using System;
using TMPro;

public class TextDisplay
{
    public TextMeshPro tMPro { get; private set; }
    public GameObject textObject { get; private set; }
    private Func<string> trackedProvider;

    internal TextDisplay(GameObject obj, TextMeshPro tm, Func<string> provider)
    {
        textObject = obj;
        tMPro = tm;
        trackedProvider = provider;
    }

    public void UpdateTrackedText()
    {
        if (trackedProvider != null && tMPro != null) tMPro.text = trackedProvider();
    }

    public void SetUpdateTracker(Func<string> provider)
    {
        trackedProvider = provider;
    }

    public void UpdateText(string text)
    {
        if (tMPro != null) tMPro.text = text;
    }

    public void UpdatePosition(Vector2 position)
    {
        if (textObject != null) textObject.transform.position = position;
    }
}
