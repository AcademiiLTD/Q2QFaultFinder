using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Image _onImage;
    [SerializeField] private Image _offImage;
    [SerializeField] private Toggle _toggle;

    private void OnEnable()
    {
        ToggleClicked();
    }

    public void ToggleClicked()
    {
        _onImage.enabled = _toggle.isOn;
        _offImage.enabled = !_toggle.isOn;
    }
}
