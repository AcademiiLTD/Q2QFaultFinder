using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CableSetupView : MonoBehaviour
{
    [SerializeField] private GameObject _introWindow, _completionWindow;
    [SerializeField] private TextMeshProUGUI _walkthroughText;

    public void ToggleIntroWindow(bool state)
    {
        _introWindow.SetActive(state);
    }

    public void ToggleCompletionWindow(bool state)
    {
        _completionWindow.SetActive(state);
    }

    public void SetWalkthroughText(string text)
    {
        _walkthroughText.text = text;
    }
}