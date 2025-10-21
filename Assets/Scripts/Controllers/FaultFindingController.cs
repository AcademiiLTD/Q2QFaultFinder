using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultFindingController : Controller
{
    [Header("Views")]
    [SerializeField] private FaultFindingView _faultFindingView;
    [SerializeField] private FinalResultPopupView _finalResultPopupView;
    [SerializeField] private List<ControllerEvent> _walkthroughControllerEvents;
    private ControllerEvent _currentWalkthroughEventListener;
    private int _walkthroughIndex = 0;
    private bool _isWalkthroughMode = false;


    private void Start()
    {
        if (PlayerPrefs.GetInt("FinishedScenario") == 0)
        {
            foreach (FaultFindingScenario scenario in GlobalData.Instance._availableFaultFindingScenarios)
            {
                PlayerPrefs.SetFloat(scenario.name, -1f);
                Debug.Log($"Set float {scenario.name} to -1f");
            }
        }    
    }

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
       switch (eventType)
        {
            case ControllerEvent.STARTED_FAULT_FINDING:
                StartNewFaultFindingScenario(false);
                _faultFindingView.SetDate(((FaultFindingScenario)eventData).date);
                break;
            case ControllerEvent.START_FAULT_FINDING_WALKTHROUGH_MODE:
                StartNewFaultFindingScenario(true);
                _faultFindingView.SetDate(((FaultFindingScenario)eventData).date);
                break;
            case ControllerEvent.SUBMIT_GUESS:
                SubmitUserGuess((UserGuess)eventData);
                break;
            case ControllerEvent.GO_TO_MAIN_MENU:
                _faultFindingView.ToggleView(false);
                _finalResultPopupView.gameObject.SetActive(false); //Doing Setactive(false) on this right now because it has an entry animation
                break;
            case ControllerEvent.CONFIRM_GUESS:
                _faultFindingView.EnableGuessConfirmationPopup();
                break;
        }

        if (_isWalkthroughMode && _walkthroughIndex > -1 &&  eventType == _walkthroughControllerEvents[_walkthroughIndex])
        {
            ProgressWalkthrough();
        }
    }

    private void StartNewFaultFindingScenario(bool walkthroughMode)
    {
        if (walkthroughMode)
        {
            _walkthroughIndex = -1;
            _isWalkthroughMode = true;
            _faultFindingView.EnableLandingPopup();
            ProgressWalkthrough();
        }
        else
        {
            _isWalkthroughMode = false;
            _faultFindingView.EnableWalkthroughContainer(-1);
        }

        _faultFindingView.ToggleView(true);
        _finalResultPopupView.gameObject.SetActive(false); //Doing Setactive(false) on this right now because it has an entry animation
    }

    public void SubmitUserGuess(UserGuess userGuess)
    {
        _finalResultPopupView.SetResultText(userGuess);
        PlayerPrefs.SetFloat($"{GlobalData.Instance.CurrentActiveScenario.name}", userGuess._userGuess);
        PlayerPrefs.SetInt("Finished Scenario", 1);
    }

    //Called from final result popup main menu button
    public void ReturnToMainMenu()
    {
        RaiseControllerEvent(ControllerEvent.GO_TO_MAIN_MENU, null);
        GlobalData.Instance.CurrentActiveScenario = null;
    }

    public void RestartCurrentScenario()
    {
        RaiseControllerEvent(ControllerEvent.STARTED_FAULT_FINDING, GlobalData.Instance.CurrentActiveScenario);
    }

    private void ProgressWalkthrough()
    {
        _walkthroughIndex++;
        Debug.Log("Walkthrough index: " + _walkthroughIndex);

        if (_walkthroughIndex < _walkthroughControllerEvents.Count) 
        {
            _currentWalkthroughEventListener = _walkthroughControllerEvents[_walkthroughIndex];
            _faultFindingView.EnableWalkthroughContainer(_walkthroughIndex);
        }
        else
        {
            _faultFindingView.EnableWalkthroughContainer(-1);
        }
    }
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
