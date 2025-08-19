using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : Controller
{
    [SerializeField] private MainMenuView _mainMenuView;

    private FaultFindingScenario _currentSelectedScenario;

    private void Start()
    {
        _mainMenuView.PopulateList(GlobalData.Instance._availableFaultFindingScenarios);
    }

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
        switch (eventType) 
        {
            case ControllerEvent.GO_TO_MAIN_MENU:
                _mainMenuView.ToggleView(true);
                break;
        }
    }

    public void ChangeSelectedScenario(int index)
    {
        _currentSelectedScenario = GlobalData.Instance._availableFaultFindingScenarios[index];
        _mainMenuView.PopulateDescriptionWindow(_currentSelectedScenario.description, _currentSelectedScenario.mapImage);
    }

    public void StartSelectedScenario()
    {
        GlobalData.Instance.CurrentActiveScenario = _currentSelectedScenario;
        RaiseControllerEvent(ControllerEvent.STARTED_FAULT_FINDING, _currentSelectedScenario);
        _mainMenuView.ToggleView(false);
    }

    public void StartCableSetup()
    {
        RaiseControllerEvent(ControllerEvent.STARTED_SETUP, null);
        _mainMenuView.ToggleView(false);
    }
}
