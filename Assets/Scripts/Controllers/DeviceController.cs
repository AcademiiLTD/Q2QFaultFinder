using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceController : Controller
{
    [SerializeField] private DeviceView _deviceView;

    [SerializeField] private List<LineSegment> _savedLineSegments;
    [SerializeField] private LineSegment _currentLineSegment;
    [SerializeField] private int _currentLineSegmentCount = 1;
    [SerializeField] private Month _selectedMonth;

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object eventData)
    {
        switch (eventType) 
        {
            case ControllerEvent.START_NEW_SECTION:
                _currentLineSegmentCount++;
                _deviceView.StartNewLineSegment(_currentLineSegmentCount);
                break;
            case ControllerEvent.SELECTED_MONTH:
                _selectedMonth = (Month)eventData;
                _deviceView.ShowCableTypeInput(_currentLineSegmentCount);
                break;
            case ControllerEvent.SELECTED_CABLE_TYPE:
                _currentLineSegment.cable = (CableType)eventData;
                _deviceView.ShowCableSizeInput();
                break;
            case ControllerEvent.SELECTED_CABLE_SIZE:
                _currentLineSegment.length = (int)eventData;
                _deviceView.ShowSectionLengthInput();
                break;
            case ControllerEvent.SUBMIT_LENGTH_INPUT:
                _currentLineSegment.length = (int)eventData;
                _savedLineSegments.Add(_currentLineSegment);
                _currentLineSegment = new LineSegment();
                RaiseControllerEvent(ControllerEvent.FINISHED_SEGMENT, _savedLineSegments);
                break;
            case ControllerEvent.FINISHED_TEST:
                DisplayFaultDistance((int)eventData);
                break;

        }
    }

    public void RestartSection()
    {
        RaiseControllerEvent(ControllerEvent.RESTART_SECTION, null);
    }

    public void StartNewTest()
    {
        RaiseControllerEvent(ControllerEvent.RESTART_TEST, null);
    }

    public void SelectMonth(int month)
    {
        Month inputMonth = (Month)month; //Cast into to month for readability
        RaiseControllerEvent(ControllerEvent.SELECTED_MONTH, month);
    }

    public void SubmitCableType(int cable)
    {
        //CableType cableType = (CableType)cable; //worry about this later
        //RaiseControllerEvent(ControllerEvent.SELECTED_CABLE_TYPE, cableType);
    }

    public void SubmitCableSize(int size)
    {
        RaiseControllerEvent(ControllerEvent.SELECTED_CABLE_SIZE, size);
    }

    public void SubmitSectionLength()
    {
        RaiseControllerEvent(ControllerEvent.SUBMIT_LENGTH_INPUT, _deviceView.KeypadValue());
    }

    private void DisplayFaultDistance(int totalLengthMinusFault)
    {
        _deviceView.ShowFinalFaultLocation(_currentLineSegmentCount, totalLengthMinusFault);
    }




}
