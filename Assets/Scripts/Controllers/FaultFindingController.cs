using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaultFindingController : Controller
{
    [Header("Views")]
    [SerializeField] private FaultFindingView _faultFindingView;
    [SerializeField] private FinalResultPopupView _finalResultPopupView;



    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
       switch (eventType)
        {
            case ControllerEvent.STARTED_FAULT_FINDING:
                StartNewFaultFindingScenario();
                break;
            case ControllerEvent.SUBMIT_GUESS:
                SubmitUserGuess((float)eventData);
                break;
            case ControllerEvent.GO_TO_MAIN_MENU:
                _faultFindingView.ToggleView(false);
                _finalResultPopupView.gameObject.SetActive(false); //Doing Setactive(false) on this right now because it has an entry animation
                break;
        }
    }

    private void StartNewFaultFindingScenario()
    {
        _faultFindingView.ToggleView(true);
        _finalResultPopupView.gameObject.SetActive(false); //Doing Setactive(false) on this right now because it has an entry animation
    }

    public void SubmitUserGuess(float userGuess)
    {
        _finalResultPopupView.SetResultText(userGuess);
        PlayerPrefs.SetFloat($"{GlobalData.Instance.CurrentActiveScenario.name}", userGuess);
    }

    //Called from final result popup main menu button
    public void ReturnToMainMenu()
    {
        RaiseControllerEvent(ControllerEvent.GO_TO_MAIN_MENU, null);
    }


    public void RestartCurrentScenario()
    {
        RaiseControllerEvent(ControllerEvent.STARTED_FAULT_FINDING, GlobalData.Instance.CurrentActiveScenario);
    }
}
