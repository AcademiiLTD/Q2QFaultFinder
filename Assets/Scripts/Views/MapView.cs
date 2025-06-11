using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private List<Color> _lineColours;
    [SerializeField] private List<ColourSelector> _colourSelectors;
    [SerializeField] private TextMeshProUGUI _previousSegmentDistanceText, _totalSegmentsDistanceText;

    public static event Action<Vector2> OnMapClicked;

    private void Awake()
    {
        PopulateColourSelectors();    
    }

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
    public void PopulateColourSelectors()
    {
        for (int i = 0; i < _lineColours.Count; i++)
        {
            _colourSelectors[i].SetColour(_lineColours[i]);
        }
    }
}
