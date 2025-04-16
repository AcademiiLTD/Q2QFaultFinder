using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CableSetupController : Controller
{
    [SerializeField] private GameObject _completionWindow;
    [SerializeField] private Transform _connectorContainer, _cableSetupContainer;
    [SerializeField] private List<GameObject> _grabbableConnectors;
    [SerializeField] private List<ConnectorPoint> _connectorPoints;
    [SerializeField] private Transform _currentGrabTarget;

    private void OnEnable()
    {
        base.OnEnable();

        Draggable.OnGrabbedDraggable += PickedUpConnector;
        Draggable.OnReleasedDraggable += ReleasedConnector;
    }

    private void OnDisable()
    {
        base.OnDisable();

        Draggable.OnGrabbedDraggable -= PickedUpConnector;
        Draggable.OnReleasedDraggable -= ReleasedConnector;
    }

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
        switch (eventType)
        {
            case ControllerEvent.STARTED_SETUP:
                BeginSetup();
                break;
            case ControllerEvent.PLACED_CONNECTOR:
                EvaluateConnectorPoints();
                break;
            case ControllerEvent.FINISHED_SETUP:
                _completionWindow.SetActive(true);
                break;
            case ControllerEvent.SUBMITTED_SCORE:

                break;
        }
    }

    private void BeginSetup()
    {
        foreach (GameObject connector in _grabbableConnectors)
        {
            connector.transform.SetParent(_connectorContainer, false);
        }
    }

    private void PickedUpConnector(Transform connector, PointerEventData data)
    {
        Debug.Log("Picked up");
        _currentGrabTarget = connector;
        _currentGrabTarget.SetParent(_cableSetupContainer, false);
    }

    private void ReleasedConnector(Transform connector, PointerEventData data)
    {
        Debug.Log("Released");

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent<ConnectorPoint>(out ConnectorPoint connectorPoint))
            {
                if (connectorPoint.Connected) break;
                _currentGrabTarget.SetParent(connectorPoint.transform, false);
                _currentGrabTarget.transform.localPosition = Vector3.zero;
                Debug.Log("Found " + connectorPoint.gameObject);
                return;
            }
        }

        _currentGrabTarget.SetParent(_connectorContainer, false);
        _currentGrabTarget = null;

    }

    private ConnectorPoint CheckForConnectorPoint()
    {
        return null;
    }

    private void EvaluateConnectorPoints()
    {
        if (CheckSetupCompletion())
        {
            RaiseControllerEvent(ControllerEvent.FINISHED_SETUP, null);
        }
    }

    private bool CheckSetupCompletion()
    {
        foreach (ConnectorPoint connectorPoint in _connectorPoints)
        {
            if (!connectorPoint.Connected)
            {
                return false;
            }
        }

        return true;
    }

    private void FinishedSetup()
    {

    }
}
