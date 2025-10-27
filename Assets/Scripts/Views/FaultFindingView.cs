using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FaultFindingView : MonoBehaviour
{
    [SerializeField] private GameObject _helpContainer, _landingPopup, _guessConfirmationPopup;
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private List<GameObject> _walkthroughContainers;

    public void ToggleHelpContainer()
    {
        if (_helpContainer.activeInHierarchy)
        {
            _helpContainer.SetActive(false);
        }
        else
        {
            _helpContainer.SetActive(true);
        }
    }

    public void EnableLandingPopup()
    {
        _landingPopup.SetActive(true);
    }

    public void SetDate(string newDate)
    {
        _dateText.text = newDate;
    }

    public void EnableWalkthroughContainer(int index)
    {
        foreach (GameObject obj in _walkthroughContainers)
        {
            obj.SetActive(false);
        }

        if (index != -1 && index < _walkthroughContainers.Count) _walkthroughContainers[index].SetActive(true);
    }

    public void EnableGuessConfirmationPopup()
    {
        _guessConfirmationPopup.SetActive(true);
    }
}
