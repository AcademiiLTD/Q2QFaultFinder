using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapView : View
{
    [SerializeField] private Image _mapBackgroundImage;
    [SerializeField] private List<Color> _lineColours;
    [SerializeField] private List<ColourSelector> _colourSelectors;
    [SerializeField] private TextMeshProUGUI _previousSegmentDistanceText, _totalSegmentsDistanceText;
    //[SerializeField] private List<LineSegmentView> _lineSegmentsDisplays;
    [SerializeField] private GameObject _lineSegmentPrefab;
    [SerializeField] private GameObject _faultAreaIndicator;

    private List<List<LineSegmentView>> _line;

    private Color _currentColour;
    private Color _previousColour;
    //private LineSegmentView _currentLineSegmentView, _previousLineSegment;
    private float _mapMetersPerPixel;

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

        /*Make sure everything is reset
        if (_currentLineSegmentView != null)
        {
            Destroy(_currentLineSegmentView.gameObject);
            _currentLineSegmentView = null;
        }

        if (_previousLineSegment != null)
        {
            Destroy(_previousLineSegment.gameObject);
            _previousLineSegment = null;
        }

        foreach (LineSegmentView lineSegmentView in _lineSegmentsDisplays)
        {
            Destroy(lineSegmentView.gameObject); //Should be pooled if performance requires
        }
        _lineSegmentsDisplays.Clear();
        */

        EvaluateSegmentLengths();
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

        List<LineSegmentView> allSegments = AllSegments();

        if (allSegments.Count == 0)
        {
            return;
        }

        LineSegmentView previousSegment = allSegments[allSegments.Count - 1];
        if (previousSegment.Color() == _currentColour)
        {
            return;
        }

        previousSegment.SetColour(_currentColour);

        List<LineSegmentView> previousColourSection = PreviousColourSection();
        previousColourSection.Remove(previousSegment);

        if (previousColourSection.Count == 0)
        {
            _line.Remove(previousColourSection);
        }

        if (_line.Count == 0)
        {
            return;
        }

        if (PreviousColourSection()[0].Color() == previousSegment.Color())
        {
            PreviousColourSection().Add(previousSegment);
        }
        else
        {
            List<LineSegmentView> newColourSection = new List<LineSegmentView>();
            newColourSection.Add(previousSegment);
            _line.Add(newColourSection);
        }
    }

    public void SetPreviousSegmentLength(float distance)
    {
        Debug.Log(distance);
        Debug.Log(_mapMetersPerPixel);
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

        LineSegmentView newLineSegment = GameObject.Instantiate(_lineSegmentPrefab, _mapBackgroundImage.transform).GetComponent<LineSegmentView>();
        newLineSegment.SetColour(_currentColour);
        newLineSegment.transform.SetAsFirstSibling();

        if (_line.Count == 0)
        {
            newLineSegment.SetFirstPosition(tapPosition, _mapMetersPerPixel, true);
            _previousColour = _currentColour;

            List<LineSegmentView> newColourSection = new List<LineSegmentView>();
            newColourSection.Add(newLineSegment);
            _line.Add(newColourSection);
        }
        else
        {
            List<LineSegmentView> allSegments = AllSegments();

            LineSegmentView previousLineSegment = allSegments[allSegments.Count - 1];

            previousLineSegment.SetSecondPosition(tapPosition);
            if (allSegments.Count > 1)
            {
                allSegments[allSegments.Count - 2].EndPointShowState(false);
            }

            newLineSegment.SetFirstPosition(previousLineSegment.EndPoisition(), _mapMetersPerPixel, false);

            EvaluateSegmentLengths();

            if (_previousColour == _currentColour)
            {
                _line[_line.Count - 1].Add(newLineSegment);
            }
            else
            {
                List<LineSegmentView> newColourSection = new List<LineSegmentView>();
                newColourSection.Add(newLineSegment);
                _line.Add(newColourSection);

                _previousColour = _currentColour;
            }

            /*
            if (newLineSegment.Color() == _line[_line.Count - 1][0].Color())
            {
                _line[_line.Count - 1].Add(newLineSegment);
            }
            else
            {
                List<LineSegmentView> newColourSection = new List<LineSegmentView>();
                newColourSection.Add(newLineSegment);
                _line.Add(newColourSection);
            }
            */
        }

        /*No line segment, this is the first tap
        if (_currentLineSegmentView == null)
        {
            _currentLineSegmentView = GameObject.Instantiate(_lineSegmentPrefab, _mapBackgroundImage.transform).GetComponent<LineSegmentView>();
            _previousLineSegment = _currentLineSegmentView;
            _lineSegmentsDisplays.Add(_currentLineSegmentView);
            _currentLineSegmentView.SetFirstPosition(tapPosition, _mapMetersPerPixel, _lineDictionary[_currentColour].Count == 0);
            _currentLineSegmentView.SetColour(_currentColour);
            _currentLineSegmentView.transform.SetAsFirstSibling();
        }
        else
        {
            _currentLineSegmentView.SetSecondPosition(tapPosition);

            EvaluateSegmentLengths();
            _currentLineSegmentView = null;
        }
        Line segment already exists, this is the second tap*/
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
            List<LineSegmentView> previousLineColourSection = _line[_line.Count - 1];

            foreach (LineSegmentView lineSegment in previousLineColourSection)
            {
                previousColourSectionLength += lineSegment.Length();
            }
        }

        /*
        foreach (LineSegmentView lineSegment in _lineSegmentsDisplays)
        {
            totalLength += lineSegment.Length();
        }

        if (_lineSegmentsDisplays.Count > 0)
        {
            _previousLineSegment = _lineSegmentsDisplays[_lineSegmentsDisplays.Count - 1];
        }
        else
        {
            _previousLineSegment = null;
        }
        */

        SetPreviousSegmentLength(previousColourSectionLength);
        SetTotalSegmentsLength(totalLength);
    }

    public void UndoSegment()
    {
        if (_line.Count > 0)
        {
            List<LineSegmentView> previousLineColourSection = _line[_line.Count - 1];

            GameObject gameObjectToDestroy = previousLineColourSection[previousLineColourSection.Count - 1].gameObject;
            previousLineColourSection.RemoveAt(previousLineColourSection.Count - 1);
            Destroy(gameObjectToDestroy);

            if (previousLineColourSection.Count == 0)
            {
                _line.Remove(previousLineColourSection);
            }

            if (_line.Count > 0)
            {
                List<LineSegmentView> allSegments = new List<LineSegmentView>();
                foreach (List<LineSegmentView> lineColourSections in _line)
                {
                    foreach (LineSegmentView lineSegmentView in lineColourSections)
                    {
                        allSegments.Add(lineSegmentView);
                    }
                }

                allSegments[allSegments.Count - 2].EndPointShowState(true);
            }
        }

        /*
        if (_lineSegmentsDisplays.Count > 0)
        {
            Destroy(_lineSegmentsDisplays[_lineSegmentsDisplays.Count - 1].gameObject);
            _lineSegmentsDisplays.RemoveAt(_lineSegmentsDisplays.Count - 1);
        }
        */

        EvaluateSegmentLengths();
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
