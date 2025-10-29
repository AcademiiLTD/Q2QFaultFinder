using System.Collections.Generic;
using UnityEngine;

public class Q2QDevice : MonoBehaviour
{
    [SerializeField] private Q2QDeviceView _deviceView;
    [SerializeField] private MapView _mapView;

    [SerializeField] private List<LineSegment> _savedLineSegments;
    [SerializeField] private LineSegment _currentLineSegment;
    [SerializeField] private int _currentLineSegmentCount = 1;
    [SerializeField] private Month _selectedMonth;
    [SerializeField] private List<CableType> _cableTypes;

    [SerializeField] private FaultFindingScenario _currentFaultFindingScenario;
    private float _faultDistanceMeters;
    private float _roundTripTime;

    private Vector2 _calculatedFaultPosition;

    private void OnEnable()
    {
        ApplicationEvents.OnScenarioSelected += OnScenarioSelected;
        ApplicationEvents.OnFaultFindingStarted += OnFaultFindingStarted;
        ApplicationEvents.OnFaultPositionCalculated += OnFaultPositionCalculated;
    }

    private void OnDisable()
    {
        ApplicationEvents.OnScenarioSelected -= OnScenarioSelected;
        ApplicationEvents.OnFaultFindingStarted -= OnFaultFindingStarted;
        ApplicationEvents.OnFaultPositionCalculated -= OnFaultPositionCalculated;
    }

    private void OnScenarioSelected(FaultFindingScenario newScenario)
    {
        _currentFaultFindingScenario = newScenario;
    }

    private void OnFaultFindingStarted()
    {
        _savedLineSegments.Clear();
        _currentLineSegment = new LineSegment();
        _currentLineSegmentCount = 1;
        _faultDistanceMeters = _currentFaultFindingScenario.faultDistance;
        _roundTripTime = CalculateRoundTripTime(_faultDistanceMeters, _currentFaultFindingScenario.LineSegments);
        _deviceView.SetDeviceActive(false);
        _deviceView.StartNewLineSegment(_currentLineSegmentCount);
        _deviceView.ShowMonthInput();
    }

    private void OnFaultPositionCalculated(Vector2 calculatedFaultPosition)
    {
        _calculatedFaultPosition = calculatedFaultPosition;
    }

    public void StartNewTest()
    {
        _savedLineSegments.Clear();
        _currentLineSegment = new LineSegment();
        _currentLineSegmentCount = 1;

        _deviceView.StartNewLineSegment(_currentLineSegmentCount);
        _deviceView.ShowMonthInput();
        _deviceView.ToggleDeviceActive();

        _mapView.ResetMap();
    }

    public void RestartSection()
    {
        _mapView.ClearCurrentColourSection();

        _currentLineSegment = new LineSegment();
        _deviceView.ShowCableTypeInput(_currentLineSegmentCount);

        _deviceView.ToggleDeviceActive();
    }

    public void SubmitMonth(int month)
    {
        _selectedMonth = (Month)month; //Cast into to month for readability
        _deviceView.ShowCableTypeInput(_currentLineSegmentCount);
    }

    public void SubmitCableType(int cableIndex)
    {
        CableType cableType = _cableTypes[cableIndex];
        _currentLineSegment.cable = cableType;
        _deviceView.ShowCableSizeInput(cableType.name.ToString());
    }

    public void SubmitCableThickness(int thickness)
    {
        _currentLineSegment.thickness = thickness;
        _deviceView.ShowSectionLengthInput();
    }

    public void SubmitSectionLength()
    {
        _currentLineSegment.length = _deviceView.KeypadValue();
        _savedLineSegments.Add(_currentLineSegment);
        Debug.Log($"Current segment count: {_currentLineSegmentCount}");

        float estimatedFaultDistance = CalculateFaultDistance(_roundTripTime, _savedLineSegments);

        if (estimatedFaultDistance == -1)
        {
            _currentLineSegment = new LineSegment();
            _currentLineSegmentCount++;
            _deviceView.StartNewLineSegment(_currentLineSegmentCount);
        }
        else
        {
            DisplayFaultDistance(estimatedFaultDistance);
        }

        _mapView.CreateNewColourSection();
    }

    private void DisplayFaultDistance(float faultDistance)
    {
        float totalDistance = 0f;

        foreach (LineSegment segment in _savedLineSegments)
        {
            totalDistance += segment.length;
        }

        ApplicationEvents.InvokeOnFaultDistanceCalculated(faultDistance);

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
        FaultPositionGuess guess = new FaultPositionGuess(_calculatedFaultPosition, _savedLineSegments, _currentFaultFindingScenario.LineSegments);
        ApplicationEvents.InvokeOnGuessSubmitted(guess);
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

        List<LineSegment> scenarioSegments = _currentFaultFindingScenario.LineSegments;

        for (int i = 0; i < segments.Count; i++)
        {
            float segmentLengthKm = segments[i].length / 1000f;
            float segmentTime = segmentLengthKm / (segments[i].cable.velocityFactor * SpeedOfLight);
            if (segments[i].thickness != scenarioSegments[i].thickness)
            {
                segmentTime *= Random.Range(1.01f, 1.1f); //Adding variance
            }

            if (oneWayTime <= segmentTime)
            {
                // Fault is in this segment
                float distanceInSegmentKm = oneWayTime * segments[i].cable.velocityFactor * SpeedOfLight;
                float distanceInSegmentM = distanceInSegmentKm * 1000f;
                float finalValue = distanceTravelled + distanceInSegmentM;
                if (_selectedMonth != _currentFaultFindingScenario.month)
                {
                    finalValue *= Random.Range(1.01f, 1.1f); //Variance
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
