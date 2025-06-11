using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioListController : Controller
{
    [SerializeField] private ScenarioListView _scenarioListView;
    [SerializeField] private List<FaultFindingScenario> _scenarios;
    private FaultFindingScenario _currentSelectedScenario;

    private void Start()
    {
        _scenarioListView.PopulateList(_scenarios);
    }

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
        
    }

    public void ChangeSelectedScenario(int index)
    {
        _currentSelectedScenario = _scenarios[index];
        _scenarioListView.PopulateDescriptionWindow(_currentSelectedScenario.description);
    }

    public void StartSelectedScenario()
    {
        RaiseControllerEvent(ControllerEvent.STARTED_FAULT_FINDING, _currentSelectedScenario);
    }

}
