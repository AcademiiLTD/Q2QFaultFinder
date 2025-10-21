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
    STARTED_SETUP,
    STARTED_FAULT_FINDING,

    GRABBED_CONNECTOR,
    RELEASED_CONNECTOR,
    PLACED_CONNECTOR,
    FINISHED_SETUP,

    START_NEW_SECTION,
    SELECTED_MONTH,
    SELECTED_CABLE_TYPE,
    SELECTED_CABLE_THICKNESS,
    TAPPED_LENGTH_INPUT,
    FINISH_KEYPAD_INPUT,
    SUBMIT_LENGTH_INPUT,
    FINISHED_SEGMENT,
    FINISHED_TEST,
    RESTART_TEST,
    RESTART_SECTION,
    SUBMIT_GUESS,
    GO_TO_MAIN_MENU,
    EVALUATE_SUBMITTED_SEGMENTS,

    START_FAULT_FINDING_WALKTHROUGH_MODE,
    CONFIRM_GUESS
}

