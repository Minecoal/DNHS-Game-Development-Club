using UnityEngine;
using System;

// This component is attached to dynamically-updated text objects so they can call back into TextDisplayer.
public class TextDisplayUpdater : MonoBehaviour
{
    private TextDisplay displayer;
    private Action onClick;

    public void Init(TextDisplay d, Func<string> trackedProvider)
    {
        displayer = d;

        if (displayer.textObject.GetComponent<Collider>() == null)
        {
            BoxCollider bc = displayer.textObject.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = new Vector3(1f, 1f, 0.1f); ;
        }

    }

    // Called by TextDisplayManager when creating objects
    public void Init(TextDisplay d)
    {
        displayer = d;
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
        if (Input.GetMouseButtonDown(0) && displayer != null && displayer.textObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == displayer.textObject.transform)
                {
                    onClick?.Invoke();
                }
            }
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
