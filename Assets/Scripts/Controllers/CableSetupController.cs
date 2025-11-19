using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CableSetupController : MonoBehaviour
{
    [SerializeField] private CableSetupView _cableSetupView;
    [SerializeField] private CanvasToggler _cableSetupCanvasToggler;

    [SerializeField] private Transform _connectorContainer, _cableSetupContainer;
    [SerializeField] private List<DraggableConnector> _grabbableConnectors;
    [SerializeField] private List<ConnectorPoint> _connectorPoints;

    [SerializeField] private AudioClip _connectionAudioClip;
    private int _cableIndex;

    private void OnEnable()
    {
        ConnectorPoint.ConnectedCorrectly += EvaluateConnectorPoints;
        ApplicationEvents.OnScenarioStarted += OnScenarioStarted;
        ApplicationEvents.OnGoToMainMenu += OnGoToMainMenu;
    }

    private void OnDisable()
    {
       ConnectorPoint.ConnectedCorrectly -= EvaluateConnectorPoints;
        ApplicationEvents.OnScenarioStarted -= OnScenarioStarted;
        ApplicationEvents.OnGoToMainMenu -= OnGoToMainMenu;

    }

    private void OnGoToMainMenu()
    {
        _cableSetupCanvasToggler.ToggleView(false);
    }

    private void OnScenarioStarted()
    {
        _cableIndex = 0;

        foreach (DraggableConnector connector in _grabbableConnectors)
        {
            connector.transform.SetParent(_connectorContainer, false);
            connector.ToggleConnectorActive(false);
        }

        _grabbableConnectors[_cableIndex].transform.localPosition = Vector3.zero;
        _grabbableConnectors[_cableIndex].ToggleConnectorActive(true);


        foreach (ConnectorPoint point in _connectorPoints)
        {
            point.ResetConnectorPoint();
        }

        _cableSetupView.SetWalkthroughText(_grabbableConnectors[_cableIndex]._hintText);
        _cableSetupView.ToggleCompletionWindow(false);
        _cableSetupView.ToggleIntroWindow(true);
        _cableSetupCanvasToggler.ToggleView(true);
    }

    //private void PickedUpConnector(Transform connector, PointerEventData data)
    //{
    //    Debug.Log("Picked up");
    //    _currentGrabTarget = connector;
    //    _currentGrabTarget.SetParent(_cableSetupContainer, false);
    //}

    //private void ReleasedConnector(Transform connector, PointerEventData data)
    //{
    //    Debug.Log("Released");

    //    DraggableConnector releasedConnector = connector.GetComponent<DraggableConnector>();
    //    ConnectorPoint connectorPoint = CheckForConnectorPoint(data);

    //    if (connectorPoint == null)
    //    {
    //        //Didn't find anything, put connector back into container
    //        _currentGrabTarget.SetParent(_connectorContainer, false);
    //        _currentGrabTarget.transform.localPosition = Vector3.zero;
    //        _currentGrabTarget = null;
    //        return;
    //    }

    //    //Found connector point, check whether they match

    //    if (connectorPoint.TryPlaceConnector(releasedConnector))
    //    {
    //        //They match, this is correct
    //        EvaluateConnectorPoints();
    //    }
    //    else
    //    {
    //        //No match, incorrect
    //        _currentGrabTarget.SetParent(_connectorContainer, false);
    //        _currentGrabTarget.transform.localPosition = Vector3.zero;
    //        _currentGrabTarget = null;
    //    }
    //}

    //private ConnectorPoint CheckForConnectorPoint(PointerEventData data)
    //{
    //    var results = new List<RaycastResult>();
    //    EventSystem.current.RaycastAll(data, results);

    //    foreach (RaycastResult result in results)
    //    {
    //        if (result.gameObject.TryGetComponent<ConnectorPoint>(out ConnectorPoint connectorPoint))
    //        {
    //            if (connectorPoint.Connected) break;
    //            Debug.Log("Found " + connectorPoint.gameObject);
    //            return connectorPoint;
    //        }
    //    }
    //    return null;
    //}

    private void EvaluateConnectorPoints()
    {
        ApplicationEvents.InvokeOnSoundEffect(_connectionAudioClip);

        if (CheckSetupCompletion())
        {
            _cableSetupView.ToggleCompletionWindow(true);
        }
    }

    private bool CheckSetupCompletion()
    {
        foreach (ConnectorPoint connectorPoint in _connectorPoints)
        {
            if (!connectorPoint.Connected)
            {
                _cableIndex++;
                _cableSetupView.SetWalkthroughText(_grabbableConnectors[_cableIndex]._hintText);

                _grabbableConnectors[_cableIndex].ToggleConnectorActive(true);
                _grabbableConnectors[_cableIndex].transform.localPosition = Vector3.zero;

                return false;
            }
        }

        return true;
    }

    public void GoToFaultFinding()
    {
        _cableSetupCanvasToggler.ToggleView(false);
        ApplicationEvents.InvokeOnFaultFinding();
    }
}
