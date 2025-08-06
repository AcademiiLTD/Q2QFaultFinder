using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CableSetupView : View
{
    [SerializeField] private GameObject _introWindow, _completionWindow;

    public void ToggleIntroWindow(bool state)
    {
        _introWindow.SetActive(state);
    }

    public void ToggleCompletionWindow(bool state)
    {
        _completionWindow.SetActive(state);
    }
}