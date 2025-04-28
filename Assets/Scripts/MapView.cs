using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI _previousSegmentDistanceText, _totalSegmentsDistanceText;

    public static event Action<Vector2> OnMapClicked;
    public void OnPointerDown(PointerEventData data)
    {
        OnMapClicked(data.position);
    }
    public void SetPreviousSegmentLength(float distance)
    {
        _previousSegmentDistanceText.text = distance.ToString();
    }

    public void SetTotalSegmentsLength(float distance)
    {
        _totalSegmentsDistanceText.text = distance.ToString();
    }
}
