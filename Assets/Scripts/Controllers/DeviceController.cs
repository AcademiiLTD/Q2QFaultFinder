using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceController : Controller
{
    [SerializeField] private DeviceView _deviceView;

    [SerializeField] private List<LineSegment> _savedLineSegments;
    [SerializeField] private LineSegment _currentLineSegment;
    [SerializeField] private int _currentLineSegmentCount = 1;
    [SerializeField] private Month _selectedMonth;
    [SerializeField] private List<CableType> _cableTypes;

    private float _faultDistanceMeters;
    private float _roundTripTime;
    private float _currentUserFaultGuess;

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
        switch (eventType) 
        {
            case ControllerEvent.STARTED_FAULT_FINDING:
                FaultFindingScenario scenario = ((FaultFindingScenario)eventData);
                _savedLineSegments.Clear();
                _currentLineSegment = new LineSegment();
                _currentLineSegmentCount = 1;
                _faultDistanceMeters = scenario.faultDistance;
                _roundTripTime = CalculateRoundTripTime(_faultDistanceMeters, ((FaultFindingScenario)eventData)._lineSegments);
                _deviceView.ManualSetDeviceActive(false);
                _deviceView.StartNewLineSegment(_currentLineSegmentCount);
                _deviceView.ShowMonthInput();
                Debug.Log(_faultDistanceMeters);
                break;
            case ControllerEvent.START_NEW_SECTION:
                _currentLineSegmentCount++;
                _deviceView.StartNewLineSegment(_currentLineSegmentCount);
                break;
            case ControllerEvent.SELECTED_MONTH:
                _selectedMonth = (Month)eventData;
                _deviceView.ShowCableTypeInput(_currentLineSegmentCount);
                break;
            case ControllerEvent.SELECTED_CABLE_TYPE:
                CableType cableType = (CableType)eventData;
                _currentLineSegment.cable = cableType;
                _deviceView.ShowCableSizeInput(cableType.name.ToString());
                break;
            case ControllerEvent.SELECTED_CABLE_THICKNESS:
                _currentLineSegment.thickness = (int)eventData;
                _deviceView.ShowSectionLengthInput();
                break;
            case ControllerEvent.SUBMIT_LENGTH_INPUT:
                _currentLineSegment.length = (int)eventData;
                _savedLineSegments.Add(_currentLineSegment);
                _currentLineSegment = new LineSegment();
                RaiseControllerEvent(ControllerEvent.FINISHED_SEGMENT, _savedLineSegments);
                break;
            case ControllerEvent.FINISHED_SEGMENT:
                float estimatedFaultDistance = CalculateFaultDistance(_roundTripTime, _savedLineSegments);
                Debug.Log($"Trip time:{_roundTripTime}");
                Debug.Log("current fault estimate: " + estimatedFaultDistance);
                if (estimatedFaultDistance == -1)
                {
                    RaiseControllerEvent(ControllerEvent.START_NEW_SECTION, null);
                }
                else
                {
                    RaiseControllerEvent(ControllerEvent.FINISHED_TEST, estimatedFaultDistance);
                }
                break;
            case ControllerEvent.FINISHED_TEST:
                DisplayFaultDistance((float)eventData);
                break;

        }
    }

    public void RestartSection()
    {
        //if (_savedLineSegments.Count > 0)
        //{
        //    _savedLineSegments.RemoveAt(_savedLineSegments.Count - 1);
        //    _currentLineSegmentCount -= 1;
        //}
        _currentLineSegment = new LineSegment();
        _deviceView.ShowCableTypeInput(_currentLineSegmentCount);
    }

    public void RestartTest()
    {
        //RaiseControllerEvent(ControllerEvent.STARTED_FAULT_FINDING, GlobalData.Instance.CurrentActiveScenario);

        _savedLineSegments.Clear();
        _currentLineSegment = new LineSegment();
        _currentLineSegmentCount = 1;
        _faultDistanceMeters = GlobalData.Instance.CurrentActiveScenario.faultDistance;
        _roundTripTime = CalculateRoundTripTime(_faultDistanceMeters, GlobalData.Instance.CurrentActiveScenario._lineSegments);
        _deviceView.StartNewLineSegment(_currentLineSegmentCount);
        _deviceView.ShowMonthInput();
        _deviceView.ManualSetDeviceActive(true);
    }

    public void SelectMonth(int month)
    {
        Month inputMonth = (Month)month; //Cast into to month for readability
        RaiseControllerEvent(ControllerEvent.SELECTED_MONTH, month);
    }

    public void SubmitCableType(int cableIndex)
    {
        CableType cableType = _cableTypes[cableIndex];
        RaiseControllerEvent(ControllerEvent.SELECTED_CABLE_TYPE, cableType);
    }

    public void SubmitCableThickness(int thickness)
    {
        RaiseControllerEvent(ControllerEvent.SELECTED_CABLE_THICKNESS, thickness);
    }

    public void SubmitSectionLength()
    {
        RaiseControllerEvent(ControllerEvent.SUBMIT_LENGTH_INPUT, _deviceView.KeypadValue());
    }

    private void DisplayFaultDistance(float faultDistance)
    {
        _currentUserFaultGuess = faultDistance;

        float totalDistance = 0f;

        foreach (LineSegment segment in _savedLineSegments)
        {
            totalDistance += segment.length;
        }

        if (_savedLineSegments.Count > 1)
        {
            for (int i = 0; i < _savedLineSegments.Count - 1; i++)
            {
                faultDistance -= _savedLineSegments[i].length;
            }
        }

        _deviceView.ShowFinalFaultLocation(_currentLineSegmentCount, faultDistance);
    }

    public void SubmitUserFaultGuess()
    {
        float finalDifference = Mathf.Abs(_faultDistanceMeters - _currentUserFaultGuess);
        UserGuess guess = new UserGuess(finalDifference, _savedLineSegments);
        RaiseControllerEvent(ControllerEvent.SUBMIT_GUESS, guess);
    }

    public float CalculateRoundTripTime(float faultDistanceMeters, List<LineSegment> segments)
    {
        float remainingDistance = faultDistanceMeters;
        float totalTimeSeconds = 0f;
        float SpeedOfLight = 299792.485f;


        foreach (LineSegment segment in segments)
        {
            //Check cable type against real scenario
            //If cable thickness is wrong, add variance
            if (remainingDistance <= segment.length)
            {
                // Fault is within this segment
                float faultInSegmentKm = remainingDistance / 1000f;
                totalTimeSeconds += faultInSegmentKm / (segment.cable.velocityFactor * SpeedOfLight);
                break;
            }
            else
            {
                float segmentKm = segment.length / 1000f;
                totalTimeSeconds += segmentKm / (segment.cable.velocityFactor * SpeedOfLight);
                remainingDistance -= segment.length;
            }
        }

        return totalTimeSeconds * 2f * 1e6f; // Convert seconds to µs for round-trip
    }

    //Used to calculate the user's final guess
    public float CalculateFaultDistance(float roundTripTimeUs, List<LineSegment> segments)
    {
        float oneWayTime = roundTripTimeUs / 2f / 1e6f; // Convert to seconds
        float distanceTravelled = 0f;
        float SpeedOfLight = 299792.485f;

        List<LineSegment> scenarioSegments = GlobalData.Instance.CurrentActiveScenario._lineSegments;

        for (int i = 0; i < segments.Count; i++)
        {
            float segmentLengthKm = segments[i].length / 1000f;
            float segmentTime = segmentLengthKm / (segments[i].cable.velocityFactor * SpeedOfLight);
            Debug.Log($"Segment before: {segmentTime}");
            if (segments[i].thickness != scenarioSegments[i].thickness)
            {
                segmentTime *= Random.Range(1.01f, 1.1f); //Adding variance
                Debug.Log($"Segment after variance: {segmentTime}");

            }



            if (oneWayTime <= segmentTime)
            {
                // Fault is in this segment
                float distanceInSegmentKm = oneWayTime * segments[i].cable.velocityFactor * SpeedOfLight;
                float distanceInSegmentM = distanceInSegmentKm * 1000f;
                float finalValue = distanceTravelled + distanceInSegmentM;
                if (_selectedMonth != GlobalData.Instance.CurrentActiveScenario.month)
                {
                    Debug.Log($"Before final variance: {finalValue}");
                    finalValue *= Random.Range(1.01f, 1.1f); //Variance
                    Debug.Log($"After final variance: {finalValue}");

                    return finalValue;
                }
                else return finalValue;
            }

            // Move to next segment
            oneWayTime -= segmentTime;
            distanceTravelled += segments[i].length;
        }

        // If fault is beyond the network
        return -1f;
    }

    public void DeviceButtonPressed()
    {
        _deviceView.ToggleDeviceActive();
    }
}
