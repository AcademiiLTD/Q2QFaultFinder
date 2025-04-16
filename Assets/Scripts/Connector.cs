using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Connector : MonoBehaviour
{
    [SerializeField] private Image _myImage;
    [SerializeField] private GameObject _label;

    public ConnectorType _connectorType;

    public void SetInteractable(bool state)
    {
        _myImage.raycastTarget = state;
    }

    public void SetVisibility(bool state)
    {
        _myImage.enabled = state;
    }

    public void ToggleLabel(bool state)
    {
        _label.SetActive(state);
    }
}
