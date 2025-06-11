using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScenarioListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _labelText;
    [SerializeField] private ScenarioListView _scenarioListView;
    private FaultFindingScenario _buttonScenario;

    public void OnClicked()
    {
        _scenarioListView.PopulateDescriptionWindow(_buttonScenario.description);
    }

    public void PopulateItem(string newText, FaultFindingScenario scenario)
    {
        _labelText.text = newText;
        _buttonScenario = scenario;
    }
}
