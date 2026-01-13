using UnityEngine;
using UnityEngine.EventSystems;
using System;

// Handles UI click/drag events for UI Text objects.
public class TextDisplayUIUpdater : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private TextDisplay displayer;
    private bool draggable = false;
    private Action onClick;

    public void Init(TextDisplay d, Func<string> trackedProvider)
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // nothing by default
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggable || displayer == null || displayer.textObject == null) return;
        RectTransform rt = displayer.textObject.GetComponent<RectTransform>();
        if (rt == null) return;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt.parent as RectTransform, eventData.position, eventData.pressEventCamera, out pos);
        rt.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // nothing by default
    }
}
