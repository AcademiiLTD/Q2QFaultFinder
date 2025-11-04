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

        StartWaitForFaultCheckPopup();
    }

    private void StartWaitForFaultCheckPopup()
    {
        StartCoroutine(WaitForFaultCheckingPopup());
    }

    private IEnumerator WaitForFaultCheckingPopup()
    {
        _faultFindingView.SetFaultCheckingPopupActive(true);

        yield return new WaitForSeconds(2.5f);

        _faultFindingView.SetFaultCheckingPopupActive(false);
        _finalResultPopupView.SetPopupActive(true);
    }

    public void RestartCurrentScenario()
    {
        ApplicationEvents.InvokeOnFaultFinding();
    }
}

[Serializable]
public class FaultPositionGuess
{
    private Vector2 _guessPosition;
    public bool _cableTypesCorrect = false, _cableThicknessCorrect = false;

    public FaultPositionGuess(Vector2 guessPosition, List<LineSegment> userInputSegments, List<LineSegment> scenarioSegments)
    {
        _guessPosition = guessPosition;
        Debug.Log($"Guess position: {guessPosition}");

        foreach (LineSegment userSegment in userInputSegments)
        {
            foreach (LineSegment scenarioSegment in scenarioSegments)
            {
                if (scenarioSegment.thickness == userSegment.thickness)
                {
                    _cableThicknessCorrect = true;
                }

                if (scenarioSegment.cable == userSegment.cable)
                {
                    _cableTypesCorrect = true;
                }
            }
        }
    }

    public Vector2 GuessPosition() => _guessPosition;
    public bool CableTypesCorrect() => _cableTypesCorrect;
    public bool CableThicknessCorrect() => _cableThicknessCorrect;
}
