using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeviceView : MonoBehaviour
{
    [SerializeField] private GameObject _cableType, _cableSize, _cableLength, _lengthKeypad, _faultDisplay;
    [SerializeField] private GameObject _consacSizeSelector, _waveconSizeSelector;
    [SerializeField] private TextMeshProUGUI _sectionCountText, _faultDisplayText;

    public void StartNewLineSegment(int segmentCount)
    {

    }

    public void ShowFinalFaultLocation(int segmentCount, int roundedMetersDistance)
    {

    }
}
