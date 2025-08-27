using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Connector : MonoBehaviour
{
    [SerializeField] private Image _myImage;
    [SerializeField] private GameObject _label;
    [TextArea]
    public string _hintText;

    public ConnectorType _connectorType;

    public void ToggleConnectorActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void ToggleLabel(bool state)
    {
        _label.SetActive(state);
    }
}
