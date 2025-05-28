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

    private void EvaluateDeviceLineSegments(List<LineSegment> segments)
    {
        //Get what the trip round time is
        //Use this to compare against the user submission
        float roundTripTime = RoundTripTime(_currentScenario._lineSegments);
        float currentTripTime = 0f;
        bool faultReached = false;

        foreach (LineSegment segment in segments)
        {
            //Calculate the travel time
            //currentTripTime += TravelTimeInSection(segment.length, segment.cable.velocityFactor);
            
            //If it's lower than what it should be, fault is further along, continue
            if (currentTripTime >= roundTripTime) faultReached = true;

        }

        if (faultReached)
        {
            //Do more stuff
        }
        else
        {
            //Ask user for another section
        }

    }

    [ContextMenu("TEST FAULT ALGORITHM")]
    public void TestFaultAlgorithm()
    {
        //This is how long the signal takes
        //This is independent of whatever the user submitted
        //We use this to compare between the real scenario and the user submission
        float roundTripTime = RoundTripTime(_currentScenario._lineSegments);
        Debug.Log($"Round trip time: {roundTripTime}");
        //float SubmittedRoundTripTime = RoundTripTime(_placeholderSubmission._lineSegments);

        Debug.Log($"Real distance: {FaultDistance(_currentScenario._lineSegments, roundTripTime)}");
        Debug.Log($"Submitted distance: {FaultDistance(_placeholderSubmission._lineSegments, roundTripTime)}");

        //Debug.Log($"Real fault trip time is {roundTripTime}, user submitted trip time is {SubmittedRoundTripTime}");
    }


    private float RoundTripTime(List<LineSegment> segments)
    {
        float roundTripTime = 0f;
        float totalDistance = 0f;
        bool foundFault = false;
        foreach (LineSegment segment in segments)
        {
            if (foundFault) break;

            totalDistance += segment.length;
            float distanceToUse = 0f;

            if (totalDistance > _currentScenario.faultDistance)
            {
                //Fault is in this section
                //Find difference and subtract before applying
                //We can also break the loop now
                distanceToUse = totalDistance - (totalDistance - _currentScenario.faultDistance);

                foundFault = true;
            }
            else
            {
                distanceToUse = segment.length;
            }

            //Debug.Log($"{totalDistance} ::: {segment.length} ::: {distanceToUse}");

            float thisSegmentTripTime = TripTimeInSection(distanceToUse, segment.cable.velocityFactor);
            roundTripTime += thisSegmentTripTime;
        }

        roundTripTime *= 2;
        roundTripTime *= 1000000;
        return roundTripTime;
    }

    private float FaultDistance(List<LineSegment> segments, float roundTripTime)
    {
        roundTripTime *= 0.5f; //Halve this to find one way trip time
        float distanceToFault = 0f; //This is the part to find

        foreach (LineSegment segment in segments)
        {

        }

        return distanceToFault;
    }

    private float TripTimeInSection(float distance, float velocityfactor)
    {
        return distance / (1000 * velocityfactor * 299792.485f);
    }

    private float DistanceBasedOnTripTime(float tripTime, float velocityFactor)
    {
        return (tripTime / 2) * velocityFactor * 299792.485f;
    }
}
