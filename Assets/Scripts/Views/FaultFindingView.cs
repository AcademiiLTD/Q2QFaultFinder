using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FaultFindingView : View
{
    [SerializeField] private GameObject _helpContainer, _landingPopup;
    [SerializeField] private TextMeshProUGUI _dateText;

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
}
