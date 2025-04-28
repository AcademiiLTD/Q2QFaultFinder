using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultFindingController : Controller
{
    [SerializeField] private GameObject _lineSegmentPrefab;
    [SerializeField] private Transform _mapTransform;
    [SerializeField] private List<LineSegmentView> _lineSegments;
    [SerializeField] private MapView _mapView;

    private LineSegmentView _currentLineSegmentView, _previousLineSegment;

    private void OnEnable()
    {
        base.OnEnable();
        MapView.OnMapClicked += TappedMap;
    }
    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
       
    }

    private void TappedMap(Vector2 tapPosition)
    {
        Debug.Log("Tapped map at " + tapPosition);

        //No line segment, this is the first tap
        if (_currentLineSegmentView == null)
        {
            _currentLineSegmentView = GameObject.Instantiate(_lineSegmentPrefab, _mapTransform).GetComponent<LineSegmentView>();
            _previousLineSegment = _currentLineSegmentView;
            _lineSegments.Add(_currentLineSegmentView);
            _currentLineSegmentView.SetFirstPosition(tapPosition);
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
        foreach (LineSegmentView lineSegment in _lineSegments)
        {
            totalLength += lineSegment.Length();
        }


        _mapView.SetPreviousSegmentLength(_previousLineSegment == null ? 0f :  _previousLineSegment.Length());
        _mapView.SetTotalSegmentsLength(totalLength);
    }

    public void UndoSegment()
    {

        if (_lineSegments.Count > 0)
        {
            Destroy(_lineSegments[_lineSegments.Count - 1].gameObject);
            _lineSegments.RemoveAt(_lineSegments.Count - 1);
        }

        if (_lineSegments.Count > 0)
        {
            _previousLineSegment = _lineSegments[_lineSegments.Count - 1];
        }
        else
        {
            _previousLineSegment = null;
        }

            EvaluateSegmentLengths();
    }
}
