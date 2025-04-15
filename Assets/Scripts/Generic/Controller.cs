using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    //Send controller event
    public static event Action<ControllerEvent, object> OnControllerEvent;

    protected void RaiseControllerEvent(ControllerEvent controllerEvent, object data)
    {
        Debug.Log("Raised event: " + controllerEvent);
        OnControllerEvent.Invoke(controllerEvent, data);
    }

    //Receive event
    protected abstract void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData);

    protected void OnEnable()
    {
        Controller.OnControllerEvent += CheckIncomingControllerEvent;
    }

    protected void OnDisable()
    {
        Controller.OnControllerEvent -= CheckIncomingControllerEvent;
    }

}

public enum ControllerEvent
{

}

