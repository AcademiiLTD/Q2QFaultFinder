using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeviceView : MonoBehaviour
{
    [SerializeField] private GameObject _monthSelector, _cableType, _cableSize, _cableLength, _faultDisplay;
    [SerializeField] private GameObject _consacSizeSelector, _waveconSizeSelector;
    [SerializeField] private TextMeshProUGUI _sectionCountText, _faultDisplayText, _keypadInputText, _sectionLengthText;
    [SerializeField] CanvasGroup _canvasGroup;

    private void Start()
    {
        ShowMonthInput();
    }
    public void SetDeviceActive(bool state)
    {
        _canvasGroup.alpha = state ? 1 : 0;
        _canvasGroup.interactable = state;
    }

    public void StartNewLineSegment(int segmentCount)
    {
        ShowCableTypeInput(segmentCount);
    }

    public void ShowMonthInput()
    {
        DisableAllScreens();
        _monthSelector.SetActive(true);
    }

    public void ShowCableTypeInput(int segmentCount)
    {
        DisableAllScreens();
        _cableType.SetActive(true);
        _sectionCountText.text = $"SELECT CABLE TYPE IN SECTION {segmentCount.ToString()}";
    }

    public void ShowCableSizeInput()
    {
        DisableAllScreens();
        _cableSize.SetActive(true);
    }

    public void ShowSectionLengthInput()
    {
        DisableAllScreens();
        _keypadInputText.text = "";
        _cableLength.SetActive(true);
    }

    public void UpdateKeypadInputField(string addition)
    {
        _keypadInputText.text += addition;
        _sectionLengthText.text = _keypadInputText.text;
    }

    public void KeypadBackspace()
    {
        string text = _keypadInputText.text;
        if (text.Length > 0) _keypadInputText.text = text.Remove(text.Length - 1);
        _sectionLengthText.text = _keypadInputText.text;

    }

    public int KeypadValue()
    {
        return int.Parse(_sectionLengthText.text);
    }

    public void ShowFinalFaultLocation(int segmentCount, float givenDistance)
    {
        DisableAllScreens();
        _faultDisplay.SetActive(true);
        _faultDisplayText.text = $"FAULT IS {givenDistance.ToString("0.00")} METERS INTO SECTION {segmentCount}";
    }

    private void DisableAllScreens()
    {
        _monthSelector.SetActive(false);
        _cableType.SetActive(false);
        _cableSize.SetActive(false);
        _cableLength.SetActive(false);
        _faultDisplay.SetActive(false);
    }
}

