using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static event Action<Transform, PointerEventData> OnGrabbedDraggable, OnReleasedDraggable;

    public void OnBeginDrag(PointerEventData data)
    {
        OnGrabbedDraggable(transform, data);
    }

    public void OnDrag(PointerEventData data)
    {
        transform.position = data.position;
    }

    public void OnEndDrag(PointerEventData data)
    {
        OnReleasedDraggable(transform, data);
    }
}
