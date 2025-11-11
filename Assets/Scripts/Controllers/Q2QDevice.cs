using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Q2QDevice : MonoBehaviour
{
    private const float MICROSECOND_SCALAR = 1e6f;
    private const float SPEED_OF_LIGHT = 299792.485f;

    [SerializeField] private Q2QDeviceView _deviceView;
    [SerializeField] private MapView _mapView;
    [SerializeField] private Button _deviceButton;
    [SerializeField] private Button _clearSectionButton;

    [SerializeField] private List<LineSegment> _savedLineSegments;
    [SerializeField] private LineSegment _currentLineSegment;
    [SerializeField] private int _visualLineSegmentCount = 1;
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
        ApplicationEvents.OnLineSectionEmpty += OnLineSectionEmpty;
    }

    private void OnDisable()
    {
        ApplicationEvents.OnScenarioSelected -= OnScenarioSelected;
        ApplicationEvents.OnFaultFindingStarted -= OnFaultFindingStarted;
        ApplicationEvents.OnFaultPositionCalculated -= OnFaultPositionCalculated;
        ApplicationEvents.OnLineSectionEmpty -= OnLineSectionEmpty;
    }

    private void OnScenarioSelected(FaultFindingScenario newScenario)
    {
        _currentFaultFindingScenario = newScenario;
    }

    private void OnFaultFindingStarted()
    {
        _savedLineSegments.Clear();
        _clearSectionButton.gameObject.SetActive(true);
        _currentLineSegment = new LineSegment();
        _visualLineSegmentCount = 1;
        _faultDistanceMeters = _currentFaultFindingScenario.faultDistance;
        _roundTripTime = CalculateRoundTripTime(_faultDistanceMeters, _currentFaultFindingScenario.LineSegments);
        _deviceView.SetDeviceActive(false);
        _deviceView.StartNewLineSegment(_visualLineSegmentCount);
        _deviceView.ShowMonthInput();
    }

    private void OnFaultPositionCalculated(Vector2 calculatedFaultPosition)
    {
        _calculatedFaultPosition = calculatedFaultPosition;
    }

    private void OnLineSectionEmpty(bool empty)
    {
        _deviceButton.gameObject.SetActive(!empty);
    }

    public void StartNewTest()
    {
        _savedLineSegments.Clear();
        _currentLineSegment = new LineSegment();
        _visualLineSegmentCount = 1;
        _clearSectionButton.gameObject.SetActive(true);

        _deviceView.StartNewLineSegment(_visualLineSegmentCount);
        _deviceView.ShowMonthInput();
        _deviceView.SetDeviceActive(false); 

        _mapView.ResetMap();
    }

    public void RestartSection()
    {
        _mapView.ClearCurrentColourSection();

        _currentLineSegment = new LineSegment();
        _deviceView.ShowCableTypeInput(_visualLineSegmentCount);

        _deviceView.SetDeviceActive(false);
    }

    public void SubmitMonth(int month)
    {
        _selectedMonth = (Month)month; //Cast into to month for readability
        _deviceView.ShowCableTypeInput(_visualLineSegmentCount);
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
        Debug.Log($"Current segment count: {_visualLineSegmentCount}");

        float estimatedFaultDistance = CalculateFaultDistance(_roundTripTime, _savedLineSegments);

        if (estimatedFaultDistance == -1)
        {
            _currentLineSegment = new LineSegment();
            _visualLineSegmentCount++;
            _deviceView.StartNewLineSegment(_visualLineSegmentCount);
            _deviceView.SetDeviceActive(false);
        }
        else
        {
            DisplayFaultDistance(estimatedFaultDistance);
            _clearSectionButton.gameObject.SetActive(false);
        }

        _mapView.CreateNewColourSection();
    }

    //Used to compound neighbouring segments that are the same type and thickness
    //Enables direct scenario to input checking in FaultPositionGuess object when user inputs multiple small segments
    private List<LineSegment> SimplifiedLineSegmentList()
    {
        if (_savedLineSegments.Count == 1)
        {
            return _savedLineSegments;
        }

        List<LineSegment> simplifiedList = _savedLineSegments;

        //Simplification occurs when segments have identical neighbours
        bool allSegmentsHaveUniqueNeighbours = false;
        while (!allSegmentsHaveUniqueNeighbours)
        {
            for (int i = 0; i < simplifiedList.Count; i++)
            {
                //If the loop reaches the end of the list without simplification, all have unique neighbours
                if (i == simplifiedList.Count - 1)
                {
                    allSegmentsHaveUniqueNeighbours = true;
                    break;
                }

                //Two segments can be simplified if they share the same type and thickness
                bool sameType = simplifiedList[i].cable == simplifiedList[i + 1].cable;
                bool sameThickness = simplifiedList[i].thickness == simplifiedList[i + 1].thickness;
                bool simplificationPossible = sameType && sameThickness;

                //Continues the loop if simplification is not possible
                if (!simplificationPossible)
                {
                    continue;
                }

                //Adds the second section length to the first, then removes the unnecessary second segment
                simplifiedList[i].length += simplifiedList[i + 1].length;
                simplifiedList.RemoveAt(i + 1);

                //Forces a break and re-loop to prevent index out of bounds error
                break;
            }
        }

        return simplifiedList;
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

        _deviceView.ShowFinalFaultLocation(_visualLineSegmentCount, faultDistance);
    }

    public void SubmitUserFaultGuess()
    {
        FaultPositionGuess guess = new FaultPositionGuess(_calculatedFaultPosition, SimplifiedLineSegmentList(), _currentFaultFindingScenario.LineSegments);
        ApplicationEvents.InvokeOnGuessSubmitted(guess);
    }

    public float CalculateRoundTripTime(float faultDistanceMeters, List<LineSegment> segments)
    {
        float remainingDistance = faultDistanceMeters;
        float totalTimeSeconds = 0f;

        foreach (LineSegment segment in segments)
        {
            //Check cable type against real scenario
            //If cable thickness is wrong, add variance
            if (remainingDistance <= segment.length)
            {
                // Fault is within this segment
                float faultInSegmentKm = remainingDistance / 1000f;
                totalTimeSeconds += faultInSegmentKm / (segment.cable.velocityFactor * SPEED_OF_LIGHT);
                break;
            }
            else
            {
                float segmentKm = segment.length / 1000f;
                totalTimeSeconds += segmentKm / (segment.cable.velocityFactor * SPEED_OF_LIGHT);
                remainingDistance -= segment.length;
            }
        }

        return totalTimeSeconds * 2f * MICROSECOND_SCALAR; // Convert seconds to µs for round-trip
    }

    //Used to calculate the user's final guess
    public float CalculateFaultDistance(float roundTripTimeUs, List<LineSegment> userSegments)
    {
        float oneWayTime = roundTripTimeUs / 2f / MICROSECOND_SCALAR; // Convert to seconds
        float distanceTravelled = 0f;

        List<LineSegment> scenarioSegments = _currentFaultFindingScenario.LineSegments;

        float variance = 1f;

        foreach (LineSegment userSegment in userSegments)
        {
            //Check this thickness against all scenario thicknesses
            //If this thickness doesn't match anything, apply variance

            bool thicknessMismatch = true;
            foreach (LineSegment scenarioSegment in scenarioSegments)
            {
                if (scenarioSegment.thickness == userSegment.thickness)
                {
                    thicknessMismatch = false;
                }
            }

            if (thicknessMismatch)
            {
                variance = variance * Random.Range(0.9f, 1.1f);
                Debug.Log("Applying thickness variance");
            }

            if (_selectedMonth != _currentFaultFindingScenario.month)
            {
                variance = variance * Random.Range(0.9f, 1.1f);
                Debug.Log("Applying month variance");
            }


            float segmentLengthKm = userSegment.length / 1000f;
            float segmentTime = segmentLengthKm / (userSegment.cable.velocityFactor * SPEED_OF_LIGHT) * variance;

            if (oneWayTime <= segmentTime)
            {
                // Fault is in this segment
                float distanceInSegmentKm = oneWayTime * userSegment.cable.velocityFactor * SPEED_OF_LIGHT;
                float distanceInSegmentM = distanceInSegmentKm * 1000f;
                float finalValue = distanceTravelled + distanceInSegmentM;

                return finalValue;
            }

            // Move to next segment
            oneWayTime -= segmentTime;
            distanceTravelled += userSegment.length;
        }

        // If fault is beyond the network
        return -1f;
    }
}
