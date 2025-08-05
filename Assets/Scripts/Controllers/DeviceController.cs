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
                _savedLineSegments.Clear();
                _currentLineSegment = null;
                _currentLineSegmentCount = 1;
                _faultDistanceMeters = ((FaultFindingScenario)eventData).faultDistance;
                _roundTripTime = CalculateRoundTripTime(_faultDistanceMeters, ((FaultFindingScenario)eventData)._lineSegments);
                _deviceView.ToggleView(false);
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
                _currentLineSegment.cable = (CableType)eventData;
                _deviceView.ShowCableSizeInput();
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
        RaiseControllerEvent(ControllerEvent.RESTART_SECTION, null);
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
        RaiseControllerEvent(ControllerEvent.SUBMIT_GUESS, finalDifference);
    }

    public static float CalculateRoundTripTime(float faultDistanceMeters, List<LineSegment> segments)
    {
        float remainingDistance = faultDistanceMeters;
        float totalTimeSeconds = 0f;
        float SpeedOfLight = 299792.485f;


        foreach (var segment in segments)
        {
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

    public static float CalculateFaultDistance(float roundTripTimeUs, List<LineSegment> segments)
    {
        float oneWayTime = roundTripTimeUs / 2f / 1e6f; // Convert to seconds
        float distanceTravelled = 0f;
        float SpeedOfLight = 299792.485f;

        foreach (var segment in segments)
        {
            float segmentLengthKm = segment.length / 1000f;
            float segmentTime = segmentLengthKm / (segment.cable.velocityFactor * SpeedOfLight);

            if (oneWayTime <= segmentTime)
            {
                // Fault is in this segment
                float distanceInSegmentKm = oneWayTime * segment.cable.velocityFactor * SpeedOfLight;
                float distanceInSegmentM = distanceInSegmentKm * 1000f;
                return distanceTravelled + distanceInSegmentM;
            }

            // Move to next segment
            oneWayTime -= segmentTime;
            distanceTravelled += segment.length;
        }

        // If fault is beyond the network
        return -1f;
    }

    public void DeviceButtonPressed()
    {
        _deviceView.ToggleDeviceActive();
    }
}
