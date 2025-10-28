using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ScenarioManifest")]
public class ScenarioManifest : ScriptableObject
{
    public List<FaultFindingScenario> FaultFindingScenarios;
}
