using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapView : MonoBehaviour
{
    [SerializeField] private Image _mapBackgroundImage;
    [SerializeField] private List<Color> _lineColours;
    [SerializeField] private List<ColourSelector> _colourSelectors;
    [SerializeField] private TextMeshProUGUI _previousSegmentDistanceText, _totalSegmentsDistanceText;
    [SerializeField] private GameObject _lineSegmentPrefab;
    [SerializeField] private GameObject _faultAreaIndicator;
    [SerializeField] private GameObject _faultGuessIndicator;
    [SerializeField] private GameObject _firstTapPositionMarker;
    [SerializeField] private GameObject _calculatedFaultArea;

    private List<List<LineSegmentView>> _line;

    private Color _currentColour;
    private Color _previousColour;
    private float _mapMetersPerPixel;

    public Vector2 FaultGuessPosition()
    {
        return _faultGuessIndicator.transform.localPosition;
    }
    private Vector2 _firstTappedPosition;
    

    private void Awake()
    {
        PopulateColourSelectors();
        _currentColour = _lineColours[0];

        _line = new List<List<LineSegmentView>>();
    }

    public void SetUpMap(Sprite mapSprite, float metersPerPixel)
    {
        _mapMetersPerPixel = metersPerPixel;
        _mapBackgroundImage.sprite = mapSprite;
        _mapBackgroundImage.gameObject.SetActive(true);

        _calculatedFaultArea.SetActive(false);

        _firstTappedPosition = Vector2.zero;

        if (_line.Count > 0)
        {
            List<LineSegmentView> allSegments = AllSegments();
            for (int i = allSegments.Count - 1; i >= 0; i--)
            {
                Destroy(allSegments[i].gameObject);
            }
        }

        _line = new List<List<LineSegmentView>>();

        EvaluateSegmentLengths();
    }

    public void SetGuessIndicatorPosition(Vector2 guessPosition)
    {
        _faultGuessIndicator.SetActive(true);
        _faultGuessIndicator.transform.position = guessPosition;
    }

    public void SetFaultAreaIndicator(Vector2 faultPosition)
    {
        //Add some random variation to the fault location so it isn't always centered on the fault
        float xPos = faultPosition.x + UnityEngine.Random.Range(-20f, 20f);
        float yPos = faultPosition.y + UnityEngine.Random.Range(-20f, 20f);

        Vector2 newPos = new Vector2(xPos, yPos);
        _faultAreaIndicator.transform.localPosition = newPos;
    }

    public void SetLineColour(int colourIndex)
    {
        _currentColour = _lineColours[colourIndex];
    }

    public void SetPreviousSegmentLength(float distance)
    {
        //Debug.Log(distance);
        //Debug.Log(_mapMetersPerPixel);
        _previousSegmentDistanceText.text = $"{distance.ToString("0.00")}m";
    }

    public void SetTotalSegmentsLength(float distance)
    {
        _totalSegmentsDistanceText.text = $"{distance.ToString("0.00")}m";
    }

    public void PopulateColourSelectors()
    {
        for (int i = 0; i < _lineColours.Count; i++)
        {
            _colourSelectors[i].SetColour(_lineColours[i]);
        }
    }

    public void PlaceLineSegment(Vector2 tapPosition)
    {
        Debug.Log("Tapped map at " + tapPosition);

        if (_firstTappedPosition == Vector2.zero)
        {
            _firstTappedPosition = tapPosition;
            _firstTapPositionMarker.transform.position = tapPosition;
            _firstTapPositionMarker.SetActive(true);
            return;
        }

        LineSegmentView newLineSegment = GameObject.Instantiate(_lineSegmentPrefab, _mapBackgroundImage.transform).GetComponent<LineSegmentView>();
        newLineSegment.SetColour(_currentColour);
        newLineSegment.transform.SetAsFirstSibling();

        if (_line.Count == 0)
        {
            newLineSegment.SetFirstPosition(_firstTappedPosition, _mapMetersPerPixel);
            newLineSegment.SetSecondPosition(tapPosition);
            _previousColour = _currentColour;

            List<LineSegmentView> newColourSection = new List<LineSegmentView>() { newLineSegment };
            _line.Add(newColourSection);
        }
        else
        {
            LineSegmentView previousLineSegment = PreviousLineSegment();
            previousLineSegment.EndPointShowState(false);

            newLineSegment.SetFirstPosition(previousLineSegment.EndPoisition(), _mapMetersPerPixel);
            newLineSegment.SetSecondPosition(tapPosition);

            if (_previousColour == _currentColour)
            {
                PreviousColourSection().Add(newLineSegment);
            }
            else
            {
                List<LineSegmentView> newColourSection = new List<LineSegmentView>() { newLineSegment };
                _line.Add(newColourSection);

                _previousColour = _currentColour;
            }
        }

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
            List<LineSegmentView> previousLineColourSection = PreviousColourSection();

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
            float colourSectionLength = 0f;
            foreach (LineSegmentView lineSegment in colourSection)
            {
                colourSectionLength += lineSegment.Length();
                lineSegment.DisableLengthLabel();
            }

            int middleLabelIndex = colourSection.Count / 2 ;
            colourSection[middleLabelIndex].SetLengthLabel(colourSectionLength);
        }
    }

    public void UndoSegment()
    {
        if (_line.Count > 0)
        {
            List<LineSegmentView> previousLineColourSection = PreviousColourSection();

            GameObject gameObjectToDestroy = PreviousLineSegment().gameObject;
            previousLineColourSection.RemoveAt(previousLineColourSection.Count - 1);
            Destroy(gameObjectToDestroy);

            if (previousLineColourSection.Count == 0)
            {
                _line.Remove(previousLineColourSection);
            }

            if (_line.Count > 0)
            {
                PreviousLineSegment().EndPointShowState(true);
            }
            else
            {
                _firstTappedPosition = Vector2.zero;
                _firstTapPositionMarker.SetActive(false);
            }
        }
        else
        {
            _firstTappedPosition = Vector2.zero;
            _firstTapPositionMarker.SetActive(false);
        }

        EvaluateSegmentLengths();
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
            return;
        }

        float distanceIntoSegment = faultDistanceFromStartMeters - accumulatedDistanceMeters;
        float ratioIntoSegment = distanceIntoSegment / targetSegment.Length();

        Vector3 segmentVector = targetSegment.EndPoisition() - targetSegment.StartPosition();
        Vector3 faultRelativeVector = segmentVector * ratioIntoSegment;

        Vector3 faultAreaPosition = targetSegment.StartPosition() + faultRelativeVector;

        _calculatedFaultArea.transform.position = faultAreaPosition;
        _calculatedFaultArea.SetActive(true);
    }

    public void HideFaultArea()
    {
        _calculatedFaultArea.SetActive(false);
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

    private LineSegmentView PreviousLineSegment()
    {
        List<LineSegmentView> allSegments = AllSegments();
        return allSegments[allSegments.Count - 1];
    }

    private List<LineSegmentView> PreviousColourSection()
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
}
