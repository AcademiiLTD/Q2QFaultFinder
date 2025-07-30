using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : Controller
{
    [SerializeField] private MapView _mapView;

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
        switch (eventType)
        {
            case ControllerEvent.STARTED_FAULT_FINDING:
                FaultFindingScenario scenario = (FaultFindingScenario)eventData;
                _mapView.SetUpMap(scenario.mapImage, scenario.mapMetersPerPixel);
                break;
        }
    }
}
