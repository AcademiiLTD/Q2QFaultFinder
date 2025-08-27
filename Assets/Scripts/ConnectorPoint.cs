using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectorPoint : MonoBehaviour
{
    [SerializeField] private Image _raycastImage;
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

    public bool TryPlaceConnector(Connector placedConnector)
    {
        if (placedConnector._connectorType == _correctConnectorType)
        {
            placedConnector.transform.SetParent(this.transform, false);
            placedConnector.transform.localPosition = Vector3.zero;
            placedConnector.ToggleConnectorActive(false);

            TogglePointConnected(true);
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
        TogglePointConnected(false);
    }

    private void TogglePointConnected(bool state)
    {
        _visual.SetActive(state);
        _raycastImage.enabled = !state;
        _connectedCorrectly = state;
    }
}

public enum ConnectorType
{
    Neutral,
    Live,
    Feed,
    Common
}