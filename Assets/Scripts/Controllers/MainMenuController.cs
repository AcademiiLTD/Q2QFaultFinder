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

    }

    public void ChangeSelectedScenario(int index)
    {
        _currentSelectedScenario = GlobalData.Instance._availableFaultFindingScenarios[index];
        _mainMenuView.PopulateDescriptionWindow(_currentSelectedScenario.description);
    }

    public void StartSelectedScenario()
    {
        RaiseControllerEvent(ControllerEvent.STARTED_FAULT_FINDING, _currentSelectedScenario);
        _mainMenuView.ShowFaultFinding();
        _currentSelectedScenario = null;
    }

    public void GoToSetupScene()
    {

    }
}
