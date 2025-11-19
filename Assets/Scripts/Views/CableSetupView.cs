using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CableSetupView : MonoBehaviour
{
    [SerializeField] private GameObject _completionWindow;
    [SerializeField] private TextMeshProUGUI _walkthroughText;
    [SerializeField] private Slideshow _introWindow;

    public void ToggleIntroWindow(bool state)
    {
        _introWindow.gameObject.SetActive(state);
        if (state) _introWindow.ResetSlideshow();
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