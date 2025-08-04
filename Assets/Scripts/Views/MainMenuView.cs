using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scenarioDescriptionWindow;
    [SerializeField] private List<ScenarioListItem> _scenarioListItems;
    [SerializeField] private GameObject _beginButton;
    [SerializeField] private CanvasGroup _mainMenuCanvasGroup, _cableSetupCanvasGroup, _faultFindingCanvasGroup;

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

    public void GoToMainMenu()
    {
        SetCanvasGroupState(_mainMenuCanvasGroup, true);
        SetCanvasGroupState(_cableSetupCanvasGroup, false);
        SetCanvasGroupState(_faultFindingCanvasGroup, false);
        
    }

    public void ShowCableSetup()
    {
        SetCanvasGroupState(_mainMenuCanvasGroup, false);
        SetCanvasGroupState(_cableSetupCanvasGroup, true);
        SetCanvasGroupState(_faultFindingCanvasGroup, false);
    }
        
    public void ShowFaultFinding()
    {
        SetCanvasGroupState(_mainMenuCanvasGroup, false);
        SetCanvasGroupState(_cableSetupCanvasGroup, false);
        SetCanvasGroupState(_faultFindingCanvasGroup, true);
    }

    private void SetCanvasGroupState(CanvasGroup canvasGroup, bool state)
    {
        canvasGroup.alpha = state ? 1 : 0;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }
}
