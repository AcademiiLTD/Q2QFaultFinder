using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private ScenarioManifest _scenarioManifest;
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private CanvasToggler _mainMenuCanvasToggler;

    private FaultFindingScenario _currentSelectedScenario;

    public void ReturnToMainMenu()
    {
        ApplicationEvents.GoToMainMenu();
        _mainMenuCanvasToggler.ToggleView(true);
    }

    private void Start()
    {
        _mainMenuView.PopulateList(_scenarioManifest.FaultFindingScenarios);
    }

    public void ChangeSelectedScenario(int index)
    {
        _currentSelectedScenario = _scenarioManifest.FaultFindingScenarios[index];
        _mainMenuView.PopulateDescriptionWindow(_currentSelectedScenario);
        ApplicationEvents.SelectScenario(_currentSelectedScenario);
    }

    public void StartSelectedScenario()
    {
        GlobalData.Instance.CurrentActiveScenario = _currentSelectedScenario;
        ApplicationEvents.ScenarioStarted();
        _mainMenuCanvasToggler.ToggleView(false);
    }
}
