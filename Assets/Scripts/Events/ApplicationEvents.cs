using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class ApplicationEvents
{
    public static UnityAction OnGoToMainMenu;
    public static void GoToMainMenu()
    {
        OnGoToMainMenu?.Invoke();
    }

    public static UnityAction<FaultFindingScenario> ScenarioSelected;
    public static void SelectScenario(FaultFindingScenario scenario)
    {
        ScenarioSelected?.Invoke(scenario);
    }

    public static UnityAction ScenarioStarted;
    public static void StartScenario()
    {
        ScenarioStarted?.Invoke();
    }
}
