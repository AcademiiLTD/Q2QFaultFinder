using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    private static GlobalData _instance;
    public static GlobalData Instance {
        get
        {
            return _instance;
        }
    }

    private FaultFindingScenario _currentActiveScenario;
    public FaultFindingScenario CurrentActiveScenario
    {
        get
        {
            return _currentActiveScenario;
        }
        set
        {
            _currentActiveScenario = value;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public float GetCurrentBestScore()
    {
        float score = PlayerPrefs.GetFloat(_currentActiveScenario.name, -1f);

        return score == -1f ? -1f : score;
    }
}
