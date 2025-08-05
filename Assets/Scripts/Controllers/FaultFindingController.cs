using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultFindingController : Controller
{
    [SerializeField] private FaultFindingScenario _currentScenario;

    [Header("Views")]
    [SerializeField] private MapView _mapView;
    [SerializeField] private DeviceView _deviceView;
    [SerializeField] private FaultFindingView _faultFindingView;
    [SerializeField] private FinalResultPopupView _finalResultPopupView;

    private void OnEnable()
    {
        base.OnEnable();
        ClickableMap.OnMapClicked += TappedMap;
    }

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
       switch (eventType)
        {
            case ControllerEvent.STARTED_FAULT_FINDING:
                _faultFindingView.ToggleView(true);
                break;
            case ControllerEvent.SUBMIT_GUESS:
                SubmitUserGuess((float)eventData);
                break;
            case ControllerEvent.GO_TO_MAIN_MENU:
                _faultFindingView.ToggleView(false);
                _finalResultPopupView.gameObject.SetActive(false);
                break;
        }
    }

    public void SubmitUserGuess(float userGuess)
    {
        _finalResultPopupView.SetResultText(userGuess);
        PlayerPrefs.SetFloat($"{_currentScenario.name}", userGuess);
    }

    private void TappedMap(Vector2 tapPosition)
    {
        _mapView.PlaceLineSegment(tapPosition);
    }
}
