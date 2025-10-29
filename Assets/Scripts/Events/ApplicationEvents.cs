using UnityEngine;
using UnityEngine.Events;

public static class ApplicationEvents
{
    public static UnityAction OnGoToMainMenu;
    public static void InvokeGoToMainMenu()
    {
        OnGoToMainMenu?.Invoke();
    }

    public static UnityAction<FaultFindingScenario> OnScenarioSelected;
    public static void InvokeOnSelectScenario(FaultFindingScenario scenario)
    {
        OnScenarioSelected?.Invoke(scenario);
    }

    public static UnityAction OnScenarioStarted;
    public static void InvokeOnStartScenario()
    {
        OnScenarioStarted?.Invoke();
    }

    public static UnityAction OnFaultFindingStarted;
    public static void InvokeOnFaultFinding()
    {
        OnFaultFindingStarted?.Invoke();
    }

    public static UnityAction<float> OnFaultDistanceCalculated;
    public static void InvokeOnFaultDistanceCalculated(float faultDistanceMeters)
    {
        OnFaultDistanceCalculated?.Invoke(faultDistanceMeters);
    }

    public static UnityAction<Vector2> OnFaultPositionCalculated;
    public static void InvokeOnFaultPositionCalculated(Vector2 faultPosition)
    {
        OnFaultPositionCalculated?.Invoke(faultPosition);
    }

    public static UnityAction<FaultPositionGuess> OnGuessSubmitted;
    public static void InvokeOnGuessSubmitted(FaultPositionGuess faultPositionGuess)
    {
        OnGuessSubmitted?.Invoke(faultPositionGuess);
    }

    public static UnityAction<bool> OnLineSectionEmpty;
    public static void InvokeOnLineSectionEmpty(bool empty)
    {
        OnLineSectionEmpty?.Invoke(empty);
    }
}
