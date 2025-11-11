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
        _finalResultPopupView.SetResultText(finalDifference, faultPositionGuess);

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
    private const float LOWER_BOUND_SCALAR = 0.9f;
    private const float UPPER_BOUND_SCALAR = 1.1f;

    private Vector2 _guessPosition;
    private bool _cableTypesCorrect = false;
    private bool _cableThicknessCorrect = false;
    private bool _cableLengthsCorrect = false;

    public FaultPositionGuess(Vector2 guessPosition, List<LineSegment> userInputSegments, List<LineSegment> scenarioSegments)
    {
        _guessPosition = guessPosition;
        //Debug.Log($"Guess position: {guessPosition}");

        //If the input count is bigger then cables have not been input correctly
        if (userInputSegments.Count != scenarioSegments.Count)
        {
            return;
        }

        //Checks one by one for matching of both thickness, type, and length
        //This prevents a user from inputting cable segments out of order
        _cableTypesCorrect = true;
        _cableThicknessCorrect = true;
        _cableLengthsCorrect = true;
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

            if (i == userInputSegments.Count - 1)
            {
                //Final segment does not need within bounds check
                break;
            }

            float lowerBound = scenarioSegments[i].length * LOWER_BOUND_SCALAR;
            bool largerThanLowerBound = lowerBound < userInputSegments[i].length;

            float upperBound = scenarioSegments[i].length * UPPER_BOUND_SCALAR;
            bool smallerThanUpperBound = userInputSegments[i].length < upperBound;

            bool withinBounds = largerThanLowerBound && smallerThanUpperBound;

            if (!withinBounds)
            {
                _cableLengthsCorrect = false;
            }
        }

        /*
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
        */
    }

    public Vector2 GuessPosition() => _guessPosition;
    public bool CableTypesCorrect() => _cableTypesCorrect;
    public bool CableThicknessCorrect() => _cableThicknessCorrect;
    public bool CableLengthsCorrect() => _cableLengthsCorrect;
}
