using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Vector2 _guessPosition;

    private List<LineSegment> _userInputSegments;
    private Sprite _userInputScreenshot;
    private List<LineSegment> _scenarioSegments;
    private Sprite _scenarioScreenshot;

    public FaultPositionGuess(
        Vector2 guessPosition,
        List<LineSegment> userInputSegments,
        Sprite userInputScreenshot,
        List<LineSegment> scenarioSegments,
        Sprite scenarioScreenshot)
    {
        _guessPosition = guessPosition;
        _userInputSegments = userInputSegments;
        _userInputScreenshot = userInputScreenshot;
        _scenarioSegments = scenarioSegments;
        _scenarioScreenshot = scenarioScreenshot;
    }

    public Vector2 GuessPosition() => _guessPosition;

    public Sprite ScenarioScreenshot() => _scenarioScreenshot;

    public Sprite UserInputScreenshot() => _userInputScreenshot;

    public int MaxCount()
    {
        if (_userInputSegments.Count > _scenarioSegments.Count)
        {
            return _userInputSegments.Count;
        }
        return _scenarioSegments.Count;
    }

    public LineSegment UserSegmentFromIndex(int index)
    {
        return SegmentFromListFromIndex(_userInputSegments, index);
    }

    public LineSegment ScenarioSegmentFromIndex(int index)
    {
        return SegmentFromListFromIndex(_scenarioSegments, index);
    }

    private LineSegment SegmentFromListFromIndex(List<LineSegment> segmentList, int index)
    {
        if (index >= segmentList.Count)
        {
            return new LineSegment();
        }
        return segmentList[index];
    }
}
