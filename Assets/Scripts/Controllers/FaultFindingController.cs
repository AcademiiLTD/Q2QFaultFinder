using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultFindingController : Controller
{
    [SerializeField] private Transform _mapTransform;
    [SerializeField] private FaultFindingScenario _currentScenario;

    [Header("Views")]
    [SerializeField] private MapView _mapView;
    [SerializeField] private DeviceView _deviceView;
    [SerializeField] private FaultFindingView _faultFindingView;

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
                //_faultFindingContainer.gameObject.SetActive(true);
                break;
            case ControllerEvent.SUBMIT_GUESS:
                SubmitUserGuess((float)eventData);
                break;
        }
    }

    public void SubmitUserGuess(float userGuess)
    {
        _faultFindingView.DisplayuserGuess(userGuess);
        PlayerPrefs.SetFloat($"{_currentScenario.name}", userGuess);
    }

    private void TappedMap(Vector2 tapPosition)
    {
        _mapView.PlaceLineSegment(tapPosition);
    }
}
