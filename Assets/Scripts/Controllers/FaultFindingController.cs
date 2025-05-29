using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultFindingController : Controller
{
    [SerializeField] private GameObject _lineSegmentPrefab;
    [SerializeField] private Transform _mapTransform;
    [SerializeField] private List<LineSegmentView> _lineSegmentsDisplays;
    [SerializeField] private MapView _mapView;
    [SerializeField] private FaultFindingScenario _currentScenario, _placeholderSubmission;
    private List<LineSegment> _inputLineSegments;

    private LineSegmentView _currentLineSegmentView, _previousLineSegment;

    private void OnEnable()
    {
        base.OnEnable();
        MapView.OnMapClicked += TappedMap;
        RaiseControllerEvent(ControllerEvent.STARTED_FAULT_FINDING, _currentScenario);
    }

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
       switch (eventType)
        {
            case ControllerEvent.STARTED_FAULT_FINDING:
                _currentScenario = (FaultFindingScenario)eventData;
                break;
            case ControllerEvent.FINISHED_SEGMENT:
                //EvaluateDeviceLineSegments((List<LineSegment>)eventData);
                break;
        }
    }

    public void SetUpScenario()
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
            _lineSegmentsDisplays.Add(_currentLineSegmentView);
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
        foreach (LineSegmentView lineSegment in _lineSegmentsDisplays)
        {
            totalLength += lineSegment.Length();
        }


        _mapView.SetPreviousSegmentLength(_previousLineSegment == null ? 0f :  _previousLineSegment.Length());
        _mapView.SetTotalSegmentsLength(totalLength);
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
