using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultFindingView : View
{
    [SerializeField] private GameObject _helpContainer;

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
}
