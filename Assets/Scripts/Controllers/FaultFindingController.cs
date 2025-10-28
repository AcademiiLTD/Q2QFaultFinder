using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultFindingController : MonoBehaviour
{

    [Header("Views")]
    [SerializeField] private CanvasToggler _faultFindingCanvasToggler;
    [SerializeField] private FaultFindingView _faultFindingView;
    [SerializeField] private FinalResultPopupView _finalResultPopupView;
    [SerializeField] private Q2QDevice _q2QDevice;
    private FaultFindingScenario _currentScenario;

    //[SerializeField] private List<ControllerEvent> _walkthroughControllerEvents;
    //private ControllerEvent _currentWalkthroughEventListener;
    //private int _walkthroughIndex = 0;
    //private bool _isWalkthroughMode = false;

    private void OnEnable()
    {
        ApplicationEvents.OnScenarioSelected += OnScenarioSelected;
        ApplicationEvents.OnGoToMainMenu += OnGoToMainMenu;
        ApplicationEvents.OnFaultFindingStarted += OnFaultFindingStarted;
        ApplicationEvents.OnGuessSubmitted += OnGuessSubmitted;
    }

    private void OnDisable()
    {
        ApplicationEvents.OnScenarioSelected -= OnScenarioSelected;
        ApplicationEvents.OnGoToMainMenu -= OnGoToMainMenu;
        ApplicationEvents.OnFaultFindingStarted -= OnFaultFindingStarted;
        ApplicationEvents.OnGuessSubmitted -= OnGuessSubmitted;
    }

    private void OnGoToMainMenu()
    {
        _faultFindingCanvasToggler.ToggleView(false);
    }

    private void OnScenarioSelected(FaultFindingScenario scenario)
    {
        _currentScenario = scenario;
        _faultFindingView.SetDate(scenario.date.ToString());
    }

    private void OnFaultFindingStarted()
    {
        Debug.Log("Fault finding started");
        _faultFindingView.EnableLandingPopup();
        _faultFindingCanvasToggler.ToggleView(true);
        _finalResultPopupView.gameObject.SetActive(false); //Doing Setactive(false) on this right now because it has an entry animation
    }

    public void OnGuessSubmitted(FaultPositionGuess faultPositionGuess)
    {
        float finalDifference = Vector2.Distance(faultPositionGuess.GuessPosition(), _currentScenario.faultPosition) / _currentScenario.mapMetersPerPixel;
        _finalResultPopupView.SetResultText(finalDifference, faultPositionGuess.CableTypesCorrect(), faultPositionGuess.CableThicknessCorrect());
    }

    public void RestartCurrentScenario()
    {
        ApplicationEvents.InvokeOnFaultFinding();
    }

    //private void ProgressWalkthrough()
    //{
    //    _walkthroughIndex++;

    //    if (_walkthroughIndex < _walkthroughControllerEvents.Count) 
    //    {
    //        _currentWalkthroughEventListener = _walkthroughControllerEvents[_walkthroughIndex];
    //        _faultFindingView.EnableWalkthroughContainer(_walkthroughIndex);
    //    }
    //    else
    //    {
    //        _faultFindingView.EnableWalkthroughContainer(-1);
    //    }
    //}
}

[Serializable]
public class FaultPositionGuess
{
    private Vector2 _guessPosition;
    public bool _cableTypesCorrect = true, _cableThicknessCorrect = true;

    public FaultPositionGuess(Vector2 guessPosition, List<LineSegment> userInputSegments, List<LineSegment> scenarioSegments)
    {
        _guessPosition = guessPosition;
        Debug.Log($"Guess position: {guessPosition}");

        for (int i = 0; i < userInputSegments.Count; i++)
        {
            if (userInputSegments[i].cable != scenarioSegments[i].cable)
            {
                _cableTypesCorrect = false;
            }

            if (userInputSegments[i].thickness != scenarioSegments[i].thickness)
            {
                _cableThicknessCorrect = false;
            }
        }
    }

    public Vector2 GuessPosition() => _guessPosition;
    public bool CableTypesCorrect() => _cableTypesCorrect;
    public bool CableThicknessCorrect() => _cableThicknessCorrect;
}
