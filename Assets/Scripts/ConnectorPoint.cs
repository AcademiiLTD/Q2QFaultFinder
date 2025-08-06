using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectorPoint : MonoBehaviour
{
    [SerializeField] private Image _myImage;
    [SerializeField] private GameObject _visual, _indicator;
    [SerializeField] private ConnectorType _correctConnectorType;
    [SerializeField] private string _hintText;
    private bool _connectedCorrectly;

    public bool Connected
    {
        get
        {
            return _connectedCorrectly;
        }
    }

    public bool TryPlaceConnector(Connector placedConnector)
    {
        if (placedConnector._connectorType == _correctConnectorType)
        {
            placedConnector.transform.SetParent(this.transform, false);
            placedConnector.transform.localPosition = Vector3.zero;
            placedConnector.SetVisibility(false);
            placedConnector.SetInteractable(false);

            _myImage.enabled = false;
            _visual.SetActive(true);
            _indicator.SetActive(false);
            _connectedCorrectly = true;
            return true;
        }
        else
        {
            //Populate dialogue
            return false;
        }
    }

    public void ResetConnectorPoint()
    {
        _myImage.enabled = true;
        _visual.SetActive(false);
        _indicator.SetActive(true);
        _connectedCorrectly = false;
    }
}

public enum ConnectorType
{
    Neutral,
    Live,
    Feed,
    Common
}