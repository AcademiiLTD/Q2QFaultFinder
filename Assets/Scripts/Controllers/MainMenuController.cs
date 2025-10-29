using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private ScenarioManifest _scenarioManifest;
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private CanvasToggler _mainMenuCanvasToggler;

    private FaultFindingScenario _currentSelectedScenario;

    public void ReturnToMainMenu()
    {
        ApplicationEvents.InvokeGoToMainMenu();
        _mainMenuCanvasToggler.ToggleView(true);
        _mainMenuView.ResetMainMenu();
    }

    private void Start()
    {
        _mainMenuView.PopulateList(_scenarioManifest.FaultFindingScenarios);
    }

    public void ChangeSelectedScenario(int index)
    {
        _currentSelectedScenario = _scenarioManifest.FaultFindingScenarios[index];
        _mainMenuView.PopulateDescriptionWindow(_currentSelectedScenario);
        ApplicationEvents.InvokeOnSelectScenario(_currentSelectedScenario);
    }

    public void StartSelectedScenario()
    {
        _mainMenuCanvasToggler.ToggleView(false);
        ApplicationEvents.InvokeOnStartScenario();
    }
}
