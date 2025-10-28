using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scenarioDescriptionText, _scenarioDateText, _scenarioNameText;
    [SerializeField] private List<ScenarioListItem> _scenarioListItems;
    [SerializeField] private GameObject _setupMenu, _faultFindingListMenu, _faultFindingDetailsMenu;

    public void PopulateDescriptionWindow(FaultFindingScenario scenario)
    {
        _scenarioDescriptionText.text = scenario.description;
        _scenarioDateText.text = scenario.date.ToString();
        _scenarioNameText.text = scenario.name;

        _faultFindingListMenu.SetActive(false);
        _faultFindingDetailsMenu.SetActive(true);
    }

    public void PopulateList(List<FaultFindingScenario> scenarios)
    {
        foreach (ScenarioListItem item in _scenarioListItems)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < scenarios.Count; i++)
        {
            _scenarioListItems[i].PopulateItem($"{scenarios[i].name}: {scenarios[i].date}");
            _scenarioListItems[i].gameObject.SetActive(true);
        }
    }

    public void ToggleMusic()
    {
        AudioSource musicSource = GetComponent<AudioSource>();
        musicSource.mute = !musicSource.mute;
    }
}
