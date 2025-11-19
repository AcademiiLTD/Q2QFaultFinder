using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapView : MonoBehaviour
{
    [SerializeField] private Image _mapBackgroundImage;
    [SerializeField] private List<Color> _lineColours;
    [SerializeField] private TextMeshProUGUI _previousSegmentDistanceText, _totalSegmentsDistanceText;
    [SerializeField] private GameObject _lineSegmentPrefab;
    [SerializeField] private GameObject _faultAreaIndicator;
    [SerializeField] private GameObject _faultGuessIndicator;
    [SerializeField] private GameObject _cableStartPositionMarker;
    [SerializeField] private GameObject _calculatedFaultArea;
    [SerializeField] private GameObject _invalidGuessPopup;

    private List<List<LineSegmentView>> _line;

    private float _mapMetersPerPixel;
    private bool _tappable;

    private void Awake()
    {
        _line = new List<List<LineSegmentView>>() { new List<LineSegmentView>() };
    }

    public void SetUpMap(FaultFindingScenario scenarioData)
    {
        _mapMetersPerPixel = scenarioData.mapMetersPerPixel;
        _mapBackgroundImage.sprite = scenarioData.mapImage;
        _mapBackgroundImage.gameObject.SetActive(true);

        _cableStartPositionMarker.transform.localPosition = scenarioData.cableStartPosition;

        //Add some random variation to the fault location so it isn't always centered on the fault
        float xPos = scenarioData.faultPosition.x + Random.Range(-125f, 125f);
        float yPos = scenarioData.faultPosition.y + Random.Range(-125f, 125f);

        Vector2 newPos = new Vector2(xPos, yPos);
        _faultAreaIndicator.transform.localPosition = newPos;

        ResetMap();
    }

    public void ResetMap()
    {
        SetTappable(true);
        CalculatedFaultAreaActive(false);
        ToggleInvalidGuessPopup(false);

        if (_line.Count > 0)
        {
            List<LineSegmentView> allSegments = AllSegments();
            for (int i = allSegments.Count - 1; i >= 0; i--)
            {
                Destroy(allSegments[i].gameObject);
            }
        }

        _line = new List<List<LineSegmentView>>() { new List<LineSegmentView>() };
        ApplicationEvents.InvokeOnLineSectionEmpty(CurrentColourSectionEmpty());
        EvaluateSegmentLengths();
    }

    public void SetGuessIndicatorPosition(Vector2 guessPosition)
    {
        _faultGuessIndicator.SetActive(true);
        _faultGuessIndicator.transform.position = guessPosition;
    }

    public void SetPreviousSegmentLength(float distance)
    {
        _previousSegmentDistanceText.text = $"{distance.ToString("0.00")}m";
    }

    public void SetTotalSegmentsLength(float distance)
    {
        _totalSegmentsDistanceText.text = $"{distance.ToString("0.00")}m";
    }

    public void PlaceLineSegment(Vector2 tapPosition)
    {
        if (!_tappable)
        {
            return;
        }

        LineSegmentView newLineSegment = GameObject.Instantiate(_lineSegmentPrefab, _mapBackgroundImage.transform).GetComponent<LineSegmentView>();
        newLineSegment.SetColour(_lineColours[_line.Count % _lineColours.Count]);
        newLineSegment.transform.SetAsFirstSibling();

        if (AllSegments().Count == 0)
        {
            Vector2 startPosition = _cableStartPositionMarker.transform.position;
            newLineSegment.SetFirstPosition(startPosition, _mapMetersPerPixel);
            newLineSegment.SetSecondPosition(tapPosition);
        }
        else
        {
            LineSegmentView previousLineSegment = MostRecentLineSegment();
            previousLineSegment.EndPointShowState(false);

            newLineSegment.SetFirstPosition(previousLineSegment.EndPoisition(), _mapMetersPerPixel);
            newLineSegment.SetSecondPosition(tapPosition);
        }

        CurrentColourSection().Add(newLineSegment);
        ApplicationEvents.InvokeOnLineSectionEmpty(CurrentColourSectionEmpty());
        EvaluateSegmentLengths();
    }

    private void EvaluateSegmentLengths()
    {
        float totalLength = 0f;
        List<LineSegmentView> allSegments = AllSegments();

        foreach (LineSegmentView lineSegment in allSegments)
        {
            totalLength += lineSegment.Length();
        }

        float previousColourSectionLength = 0f;
        if (_line.Count > 0)
        {
            List<LineSegmentView> previousLineColourSection = CurrentColourSection();

            foreach (LineSegmentView lineSegment in previousLineColourSection)
            {
                previousColourSectionLength += lineSegment.Length();
            }
        }

        SetPreviousSegmentLength(previousColourSectionLength);
        SetTotalSegmentsLength(totalLength);
        UpdateSectionLabels();
    }

    private void UpdateSectionLabels()
    {
        foreach (List<LineSegmentView> colourSection in _line)
        {
            if (colourSection.Count == 0)
            {
                continue;
            }

            float colourSectionLength = 0f;
            foreach (LineSegmentView lineSegment in colourSection)
            {
                colourSectionLength += lineSegment.Length();
                lineSegment.DisableLengthLabel();
            }

            int middleLabelIndex = colourSection.Count / 2;
            colourSection[middleLabelIndex].SetLengthLabel(colourSectionLength);
        }
    }

    public void UndoLineSegment()
    {
        if (CurrentColourSectionEmpty())
        {
            return;
        }

        LineSegmentView mostRecentLineSegment = MostRecentLineSegment();
        CurrentColourSection().Remove(mostRecentLineSegment);
        Destroy(mostRecentLineSegment.gameObject);

        if (AllSegments().Count == 0)
        {
            return;
        }

        MostRecentLineSegment().EndPointShowState(true);
    }

    public void ClearCurrentColourSection()
    {
        List<LineSegmentView> currentSection = CurrentColourSection();

        foreach (LineSegmentView lineSegmentView in currentSection)
        {
            Destroy(lineSegmentView.gameObject);
        }

        currentSection.Clear();

        if (AllSegments().Count > 0)
        {
            MostRecentLineSegment().EndPointShowState(true);
        }

        ApplicationEvents.InvokeOnLineSectionEmpty(CurrentColourSectionEmpty());
        EvaluateSegmentLengths();
    }

    public void CreateNewColourSection()
    {
        List<LineSegmentView> newColourSection = new List<LineSegmentView>();
        _line.Add(newColourSection);
        ApplicationEvents.InvokeOnLineSectionEmpty(CurrentColourSectionEmpty());
    }

    public void DisplayFaultArea(float faultDistanceFromStartMeters)
    {
        List<LineSegmentView> allSegments = AllSegments();

        float accumulatedDistanceMeters = 0f;
        LineSegmentView targetSegment = null;

        for (int i = 0; i < allSegments.Count; i++)
        {
            if (accumulatedDistanceMeters + allSegments[i].Length() < faultDistanceFromStartMeters)
            {
                accumulatedDistanceMeters += allSegments[i].Length();
                continue;
            }

            targetSegment = allSegments[i];
            break;
        }

        if (targetSegment == null)
        {
            SetTappable(false);
            _invalidGuessPopup.SetActive(true);
            return;
        }

        float distanceIntoSegment = faultDistanceFromStartMeters - accumulatedDistanceMeters;
        float ratioIntoSegment = distanceIntoSegment / targetSegment.Length();

        Vector3 segmentVector = targetSegment.EndPoisition() - targetSegment.StartPosition();
        Vector3 faultRelativeVector = segmentVector * ratioIntoSegment;

        Vector3 faultAreaPosition = targetSegment.StartPosition() + faultRelativeVector;

        _calculatedFaultArea.transform.position = faultAreaPosition;
        _calculatedFaultArea.SetActive(true);

        ApplicationEvents.InvokeOnFaultPositionCalculated(_calculatedFaultArea.transform.localPosition);
    }

    private List<LineSegmentView> AllSegments()
    {
        List<LineSegmentView> allSegments = new List<LineSegmentView>();
        foreach (List<LineSegmentView> lineColourSections in _line)
        {
            foreach (LineSegmentView lineSegmentView in lineColourSections)
            {
                allSegments.Add(lineSegmentView);
            }
        }

        return allSegments;
    }

    private LineSegmentView MostRecentLineSegment()
    {
        List<LineSegmentView> allSegments = AllSegments();
        return allSegments[allSegments.Count - 1];
    }

    private List<LineSegmentView> CurrentColourSection()
    {
        if (_line.Count > 0)
        {
            return _line[_line.Count - 1];
        }
        else
        {
            return new List<LineSegmentView>();
        }
    }

    private bool CurrentColourSectionEmpty()
    {
        return CurrentColourSection().Count == 0;
    }

    public void SetTappable(bool tappable)
    { 
        _tappable = tappable;
    }

    public void CalculatedFaultAreaActive(bool shown)
    {
        _calculatedFaultArea.SetActive(shown);
    }

    public void ToggleInvalidGuessPopup(bool state)
    {
        _invalidGuessPopup.SetActive(state);
    }
}
