using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapView : View
{
    [SerializeField] private Image _mapBackgroundImage;
    [SerializeField] private List<Color> _lineColours;
    [SerializeField] private List<ColourSelector> _colourSelectors;
    [SerializeField] private TextMeshProUGUI _previousSegmentDistanceText, _totalSegmentsDistanceText;
    [SerializeField] private List<LineSegmentView> _lineSegmentsDisplays;
    [SerializeField] private GameObject _lineSegmentPrefab;

    private Color _currentColour;
    private List<LineSegment> _inputLineSegments;
    private LineSegmentView _currentLineSegmentView, _previousLineSegment;
    private float _mapMetersPerPixel;

    private void Awake()
    {
        PopulateColourSelectors();
        _currentColour = _lineColours[0];
    }

    public void SetUpMap(Sprite mapSprite, float metersPerPixel)
    {
        _mapMetersPerPixel = metersPerPixel;
        _mapBackgroundImage.sprite = mapSprite;
        _mapBackgroundImage.gameObject.SetActive(true);
    }

    public void SetLineColour(int colourIndex)
    {
        _currentColour = _lineColours[colourIndex];
    }

    public void SetPreviousSegmentLength(float distance)
    {
        Debug.Log(distance);
        Debug.Log(_mapMetersPerPixel);
        float scaledDistance = distance / _mapMetersPerPixel;
        _previousSegmentDistanceText.text = $"{scaledDistance.ToString("0.00")}m";
        _previousLineSegment.SetLength($"{scaledDistance.ToString("0.00")}m");
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

    public void PlaceLineSegment(Vector2 tapPosition)
    {
        Debug.Log("Tapped map at " + tapPosition);

        //No line segment, this is the first tap
        if (_currentLineSegmentView == null)
        {
            _currentLineSegmentView = GameObject.Instantiate(_lineSegmentPrefab, _mapBackgroundImage.transform).GetComponent<LineSegmentView>();
            _previousLineSegment = _currentLineSegmentView;
            _lineSegmentsDisplays.Add(_currentLineSegmentView);
            _currentLineSegmentView.SetFirstPosition(tapPosition);
            _currentLineSegmentView.SetColour(_currentColour);
            _currentLineSegmentView.transform.SetAsFirstSibling();
        }
        else
        {
            _currentLineSegmentView.SetSecondPosition(tapPosition);

            EvaluateSegmentLengths();
            _currentLineSegmentView = null;
        }
        //Line segment already exists, this is the second tap
    }

    private void EvaluateSegmentLengths()
    {
        float totalLength = 0f;
        foreach (LineSegmentView lineSegment in _lineSegmentsDisplays)
        {
            totalLength += lineSegment.Length();
        }


        if (_previousLineSegment == null) return;
        SetPreviousSegmentLength(_previousLineSegment == null ? 0f : _previousLineSegment.Length());
        SetTotalSegmentsLength(totalLength);
    }

    public void UndoSegment()
    {

        if (_lineSegmentsDisplays.Count > 0)
        {
            Destroy(_lineSegmentsDisplays[_lineSegmentsDisplays.Count - 1].gameObject);
            _lineSegmentsDisplays.RemoveAt(_lineSegmentsDisplays.Count - 1);
        }

        if (_lineSegmentsDisplays.Count > 0)
        {
            _previousLineSegment = _lineSegmentsDisplays[_lineSegmentsDisplays.Count - 1];
        }
        else
        {
            _previousLineSegment = null;
        }

        EvaluateSegmentLengths();
    }
}
