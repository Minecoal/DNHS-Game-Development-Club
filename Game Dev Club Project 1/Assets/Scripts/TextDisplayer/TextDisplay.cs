using UnityEngine;
using System;
using UnityEngine.UI;

public class TextDisplay
{
    // Supports both TextMesh (3D) and UnityEngine.UI.Text (UI)
    public Component textComponent { get; private set; }
    public GameObject textObject { get; private set; }
    private Func<string> trackedProvider;

    internal TextDisplay(GameObject obj, Component txtComp, Func<string> provider)
    {
        textObject = obj;
        textComponent = txtComp;
        trackedProvider = provider;
    }

    public void UpdateTrackedText()
    {
        if (trackedProvider == null || textComponent == null) return;
        string s = trackedProvider();
        if (textComponent is TextMesh tm)
            tm.text = s;
        else if (textComponent is Text ui)
            ui.text = s;
    }

    public void SetUpdateTracker(Func<string> provider)
    {
        trackedProvider = provider;
    }

    public void UpdateText(string text)
    {
        if (textComponent == null) return;
        if (textComponent is TextMesh tm)
            tm.text = text;
        else if (textComponent is Text ui)
            ui.text = text;
    }

    public void UpdatePosition(Vector3 position)
    {
        if (textObject != null) textObject.transform.position = position;
    }
}
