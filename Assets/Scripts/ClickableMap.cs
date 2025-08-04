using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableMap : MonoBehaviour, IPointerDownHandler
{
    public static event Action<Vector2> OnMapClicked;

    public void OnPointerDown(PointerEventData data)
    {
        OnMapClicked(data.position);
    }
}
