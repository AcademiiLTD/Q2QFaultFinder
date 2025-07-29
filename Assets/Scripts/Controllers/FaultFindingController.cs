using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultFindingController : Controller
{
    [SerializeField] private List<Color> _availableColours;
    [SerializeField] private Transform _mapTransform;

    [SerializeField] private MapView _mapView;
    [SerializeField] private FaultFindingScenario _currentScenario;
    [SerializeField] private GameObject _faultFindingContainer;
    [SerializeField] private FinalResultPopupView _finalResultView;

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
       switch (eventType)
        {
            case ControllerEvent.STARTED_FAULT_FINDING:
                _faultFindingContainer.gameObject.SetActive(true);
                _currentScenario = (FaultFindingScenario)eventData;
                break;
            case ControllerEvent.SUBMIT_GUESS:
                SubmitUserGuess((float)eventData);
                break;
        }
    }

    public void SubmitUserGuess(float userGuess)
    {
        _finalResultView.SetResultText(userGuess);
        PlayerPrefs.SetString($"{_currentScenario.name}", $"{userGuess.ToString()}");
    }
}
