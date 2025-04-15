using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Draggable : MonoBehaviour, IDragHandler
{
    public UnityEvent OnStartedDrag, OnFinishedDrag;

    public void OnBeginDrag(PointerEventData data)
    {
        OnStartedDrag.Invoke();
    }

    public void OnDrag(PointerEventData data)
    {
        transform.position = data.position;
    }

    public void OnEndDrag(PointerEventData data)
    {
        OnFinishedDrag.Invoke();
    }
}
