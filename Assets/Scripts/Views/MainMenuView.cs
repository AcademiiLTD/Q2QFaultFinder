using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuView : View
{
    [SerializeField] private TextMeshProUGUI _scenarioDescriptionWindow;
    [SerializeField] private List<ScenarioListItem> _scenarioListItems;
    [SerializeField] private GameObject _beginButton;

    public void PopulateDescriptionWindow(string description)
    {
        _scenarioDescriptionWindow.text = description;
        _beginButton.SetActive(true);
    }

    public void PopulateList(List<FaultFindingScenario> scenarios)
    {
        foreach (ScenarioListItem item in _scenarioListItems)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < scenarios.Count; i++)
        {
            _scenarioListItems[i].PopulateItem($"{scenarios[i].name}:{scenarios[i].date}");
            _scenarioListItems[i].gameObject.SetActive(true);
        }
    }
}
