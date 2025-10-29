using UnityEngine;
using System;
using TMPro;

public class TextDisplay
{
    // Use the TMP_Text base class so this supports both TextMeshPro (3D) and TextMeshProUGUI (UI)
    public TMP_Text tMPro { get; private set; }
    public GameObject textObject { get; private set; }
    private Func<string> trackedProvider;

    internal TextDisplay(GameObject obj, TMP_Text tm, Func<string> provider)
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

    public void UpdatePosition(Vector3 position)
    {
        if (textObject != null) textObject.transform.position = position;
    }
}
