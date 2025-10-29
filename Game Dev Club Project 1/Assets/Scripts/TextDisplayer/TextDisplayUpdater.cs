using UnityEngine;
using System;

// This component is attached to dynamically-updated text objects so they can call back into TextDisplayer.
public class TextDisplayUpdater : MonoBehaviour
{
    private TextDisplay displayer;
    private Func<string> provider;
    private bool draggable = false;
    private Action onClick;
    private bool isDragging = false;
    private Vector3 dragOffset;

    public void Init(TextDisplay d, Func<string> trackedProvider)
    {
        displayer = d;
        provider = trackedProvider;
    }

    // Called by TextDisplayManager when creating objects
    public void Init(TextDisplay d)
    {
        displayer = d;
    }

    public void SetDraggable(bool value)
    {
        draggable = value;
    }

    public void SetOnClick(Action callback)
    {
        onClick = callback;
    }

    void Update()
    {
        if (displayer != null)
        {
            displayer.UpdateTrackedText();
        }

        // Click/drag handling
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            if (displayer != null && displayer.textObject != null)
            {
                float dist = Vector3.Distance(mouseWorld, displayer.textObject.transform.position);
                if (dist < 0.5f)
                {
                    // clicked
                    onClick?.Invoke();
                    if (draggable && !isDragging)
                    {
                        isDragging = true;
                        dragOffset = displayer.textObject.transform.position - mouseWorld;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && draggable && displayer != null && displayer.textObject != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            displayer.textObject.transform.position = mouseWorld + dragOffset;
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
