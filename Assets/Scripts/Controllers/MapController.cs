using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : Controller
{
    [SerializeField] private MapView _mapView;
    private bool _userMakingFaultGuess;

    private void OnEnable()
    {
        base.OnEnable();
        ClickableMap.OnMapClicked += TappedMap;
    }

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
        switch (eventType)
        {
            case ControllerEvent.START_FAULT_FINDING_WALKTHROUGH_MODE:
            case ControllerEvent.STARTED_FAULT_FINDING:
                FaultFindingScenario scenario = (FaultFindingScenario)eventData;
                _mapView.SetUpMap(scenario.mapImage, scenario.mapMetersPerPixel);
                _mapView.SetFaultAreaIndicator(scenario.faultPosition);
                break;
        }
    }

    public void ToggleGuess()
    {
        _userMakingFaultGuess = !_userMakingFaultGuess;
    }

    private void TappedMap(Vector2 tapPosition)
    {
        if (_userMakingFaultGuess)
        {
            //Need to submit the user's guess instead of placing a line segment
            RaiseControllerEvent(ControllerEvent.CONFIRM_GUESS, tapPosition);
            _mapView.SetGuessIndicatorPosition(tapPosition);
            return;
        }

        _mapView.PlaceLineSegment(tapPosition);
    }

    public void UserSubmitGuess()
    {
        FaultFindingScenario scenario = GlobalData.Instance.CurrentActiveScenario;
        Vector2 guessPosition = _mapView.FaultGuessPosition();
        RaiseControllerEvent(ControllerEvent.SUBMIT_GUESS, guessPosition);
    }
}
