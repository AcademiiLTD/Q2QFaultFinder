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
        ApplicationEvents.ScenarioSelected += PopulateScenario;
        ApplicationEvents.OnGoToMainMenu += LeaveFaultFinding;
    }

    private void OnDisable()
    {
        ApplicationEvents.ScenarioSelected -= PopulateScenario;
        ApplicationEvents.OnGoToMainMenu -= LeaveFaultFinding;

    }

    private void LeaveFaultFinding()
    {
        _faultFindingCanvasToggler.ToggleView(false);
    }

    private void PopulateScenario(FaultFindingScenario scenario)
    {

    }

    private void StartNewFaultFindingScenario(bool walkthroughMode)
    {
        _faultFindingView.EnableLandingPopup();
        _faultFindingCanvasToggler.ToggleView(true);
        _finalResultPopupView.gameObject.SetActive(false); //Doing Setactive(false) on this right now because it has an entry animation
    }

    public void SubmitUserGuess(Vector2 userGuess)
    {
        FaultFindingScenario scenario = GlobalData.Instance.CurrentActiveScenario;
        float finalDifference = Vector2.Distance(userGuess, scenario.faultPosition) / scenario.mapMetersPerPixel;

        _finalResultPopupView.SetResultText(finalDifference);
        PlayerPrefs.SetFloat($"{GlobalData.Instance.CurrentActiveScenario.name}", finalDifference);
        PlayerPrefs.SetInt("Finished Scenario", 1);
    }

    public void RestartCurrentScenario()
    {
        //RaiseControllerEvent(ControllerEvent.STARTED_FAULT_FINDING, GlobalData.Instance.CurrentActiveScenario);
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
public class UserGuess
{
    public float _userGuess;
    public bool _cableTypesCorrect = true, _cableThicknessCorrect = true;

    public UserGuess(float guess, List<LineSegment> segments)
    {
        _userGuess = guess;

        List<LineSegment> scenarioSegments = GlobalData.Instance.CurrentActiveScenario._lineSegments;

        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].cable != scenarioSegments[i].cable)
            {
                _cableTypesCorrect = false;
            }

            if (segments[i].thickness != scenarioSegments[i].thickness)
            {
                _cableThicknessCorrect = false;
            }
        }
    }
}
