using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CableSetupController : Controller
{
    [SerializeField] private CableSetupView _cableSetupView;
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
                _cableSetupView.ToggleView(true);
                BeginSetup();
                break;
            case ControllerEvent.FINISHED_SETUP:
                _completionWindow.SetActive(true);
                break;
            case ControllerEvent.GO_TO_MAIN_MENU:
                _cableSetupView.ToggleView(false);
                break;
        }
    }

    private void BeginSetup()
    {
        foreach (GameObject connector in _grabbableConnectors)
        {
            connector.transform.SetParent(_connectorContainer, false);
            Image connectorImage = connector.GetComponent<Image>();
            connectorImage.enabled = true;
            connectorImage.raycastTarget = true;
        }

        foreach (ConnectorPoint point in _connectorPoints)
        {
            point.ResetConnectorPoint();
        }

        _completionWindow.gameObject.SetActive(false);

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

        Connector releasedConnector = connector.GetComponent<Connector>();
        ConnectorPoint connectorPoint = CheckForConnectorPoint(data);

        if (connectorPoint == null)
        {
            //Didn't find anything, put connector back into container
            _currentGrabTarget.SetParent(_connectorContainer, false);
            _currentGrabTarget = null;
            return;
        }

        //Found connector point, check whether they match

        if (connectorPoint.TryPlaceConnector(releasedConnector))
        {
            //They match, this is correct
            EvaluateConnectorPoints();
        }
        else
        {
            //No match, incorrect
            _currentGrabTarget.SetParent(_connectorContainer, false);
            _currentGrabTarget = null;
        }
        

    }

    private ConnectorPoint CheckForConnectorPoint(PointerEventData data)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent<ConnectorPoint>(out ConnectorPoint connectorPoint))
            {
                if (connectorPoint.Connected) break;
                Debug.Log("Found " + connectorPoint.gameObject);
                return connectorPoint;
            }
        }
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

    public void ReturnToMainMenu()
    {
        RaiseControllerEvent(ControllerEvent.GO_TO_MAIN_MENU, null);
    }
}
