using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private MapView _mapView;
    [SerializeField] private AudioClip _touchscreenTapAudioClip;
    private bool _userMakingFaultGuess;

    private void OnEnable()
    {
        ClickableMap.OnMapClicked += TappedMap;

        ApplicationEvents.OnScenarioSelected += OnScenarioSelected;
        ApplicationEvents.OnFaultFindingStarted += OnFaultFindingStarted;
        ApplicationEvents.OnFaultDistanceCalculated += OnFaultDistanceCalculated;
    }

    private void OnDisable()
    {
        ClickableMap.OnMapClicked -= TappedMap;

        ApplicationEvents.OnScenarioSelected -= OnScenarioSelected;
        ApplicationEvents.OnFaultFindingStarted -= OnFaultFindingStarted;
        ApplicationEvents.OnFaultDistanceCalculated -= OnFaultDistanceCalculated;
    }

    private void OnScenarioSelected(FaultFindingScenario scenarioData)
    {
        _mapView.SetUpMap(scenarioData);
    }

    private void OnFaultFindingStarted()
    {
        _mapView.ResetMap();
    }

    private void OnFaultDistanceCalculated(float faultDistanceFromStartMeters)
    {
        _mapView.DisplayFaultArea(faultDistanceFromStartMeters);
    }

    private void TappedMap(Vector2 tapPosition)
    {
        if (_userMakingFaultGuess)
        {
            //Need to submit the user's guess instead of placing a line segment
            _mapView.SetGuessIndicatorPosition(tapPosition);
            return;
        }

        _mapView.PlaceLineSegment(tapPosition);
        ApplicationEvents.InvokeOnSoundEffect(_touchscreenTapAudioClip);
    }
}
