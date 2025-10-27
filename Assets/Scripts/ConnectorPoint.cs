using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConnectorPoint : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _collider;
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

    public static UnityAction ConnectedCorrectly;

    public void TryPlaceConnector(DraggableConnector placedConnector)
    {
        if (placedConnector._connectorType == _correctConnectorType)
        {
            placedConnector.ToggleConnectorActive(false);
            TogglePointConnected(true);
            ConnectedCorrectly?.Invoke();
        }
        else
        {
            placedConnector.ResetConnectorPosition();
        }
    }

    public void ResetConnectorPoint()
    {
        TogglePointConnected(false);
    }

    private void TogglePointConnected(bool state)
    {
        _visual.SetActive(state);
        _collider.enabled = !state;
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