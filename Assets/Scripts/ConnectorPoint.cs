using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectorPoint : MonoBehaviour
{
    [SerializeField] private Image _myImage;
    [SerializeField] private GameObject _visual;
    [SerializeField] private ConnectorType _correctConnectorType;
    private bool _connectedCorrectly;

    public bool Connected
    {
        get
        {
            return _connectedCorrectly;
        }
    }

    public bool PlacedConnectorIsCorrect(ConnectorType connectorType)
    {
        if (connectorType == _correctConnectorType)
        {
            _visual.SetActive(true);
            return true;
        }
        else
        {
            return false;
        }
    }
}

public enum ConnectorType
{
    One,
    Two,
    Three,
    Four
}