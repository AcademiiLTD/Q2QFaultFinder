using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FinalResultPopupView : MonoBehaviour
{
    private const string TYPE_STRING = "Cable Type: ";
    private const string THICKNESS_STRING = "Cable Thickness: ";
    private const string LENGTH_STRING = "Cable Length: ";
    private const string NOT_APPLICABLE = "N/A";

    private const float CORRECT_BOUND = 0.1f;
    private const float MEDIAN_BOUND = 0.2f;

    private const string SEGMENT_STRING = "Segment: ";

    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Color _correctColour, _medianColour, _incorrectColour;

    [SerializeField] private TextMeshProUGUI _segmentNumberLabel;

    [Header("User Input Comparison")]
    [SerializeField] private Image _userInputScreenshot;
    [SerializeField] private TextMeshProUGUI _userInputCableTypeLabel;
    [SerializeField] private TextMeshProUGUI _userInputCableThicknessLabel;
    [SerializeField] private TextMeshProUGUI _userInputCableLengthLabel;

    [Header("Scenario Comparison")]
    [SerializeField] private Image _scenarioScreenshot;
    [SerializeField] private TextMeshProUGUI _scenarioCableTypeLabel;
    [SerializeField] private TextMeshProUGUI _scenarioCableThicknessLabel;
    [SerializeField] private TextMeshProUGUI _scenarioCableLengthLabel;

    private FaultPositionGuess _faultPositionGuess;
    private int _currentComparisonIndex;

    public void SetResultText(float userGuess, FaultPositionGuess faultPositionGuess)
    {
        _faultPositionGuess = faultPositionGuess;

        Color textColour = Color.black;
        switch (userGuess)
        {
            case > 10f:
                textColour = _incorrectColour;
                break;
            case > 5f:
                textColour = _medianColour;
                break;
            case <= 3f:
                textColour = _correctColour;
                break;
        }

        string hexColour = textColour.ToHexString();

        _resultText.text = $"Your guess was <color=#{hexColour}>{userGuess.ToString("0.00")}</color> meters from the fault";

        _scenarioScreenshot.sprite = faultPositionGuess.ScenarioScreenshot();
        _userInputScreenshot.sprite = faultPositionGuess.UserInputScreenshot();

        _currentComparisonIndex = 0;
        DisplayComparison();
    }

    private void DisplayComparison()
    {
        _segmentNumberLabel.text = SEGMENT_STRING + (_currentComparisonIndex + 1).ToString();

        LineSegment userSegment = _faultPositionGuess.UserSegmentFromIndex(_currentComparisonIndex);
        LineSegment scenarioSegment = _faultPositionGuess.ScenarioSegmentFromIndex(_currentComparisonIndex);

        string userCableType = (userSegment.cable != null) ? TYPE_STRING + userSegment.cable.name : TYPE_STRING + NOT_APPLICABLE;
        string userCableThickness = (userSegment.thickness != 0f) ? THICKNESS_STRING + userSegment.thickness.ToString() : THICKNESS_STRING + NOT_APPLICABLE;
        string userCableLength = (userSegment.length != 0f) ? LENGTH_STRING + userSegment.length.ToString() : LENGTH_STRING + NOT_APPLICABLE;

        _userInputCableTypeLabel.text = userCableType;
        _userInputCableThicknessLabel.text = userCableThickness;
        _userInputCableLengthLabel.text = userCableLength;

        string scenarioCableType = (scenarioSegment.cable != null) ? TYPE_STRING + scenarioSegment.cable.name : TYPE_STRING + NOT_APPLICABLE;
        string scenarioCableThickness = (scenarioSegment.thickness != 0f) ? THICKNESS_STRING + scenarioSegment.thickness.ToString() : THICKNESS_STRING + NOT_APPLICABLE;
        string scenarioCableLength = (scenarioSegment.length != 0f) ? LENGTH_STRING + scenarioSegment.length.ToString() : LENGTH_STRING + NOT_APPLICABLE;

        _scenarioCableTypeLabel.text = scenarioCableType;
        _scenarioCableThicknessLabel.text = scenarioCableThickness;
        _scenarioCableLengthLabel.text = scenarioCableLength;

        if (userSegment.cable != scenarioSegment.cable)
        {
            _userInputCableTypeLabel.color = _incorrectColour;
        }
        else
        {
            _userInputCableTypeLabel.color = _correctColour;
        }

        if (userSegment.thickness != scenarioSegment.thickness)
        {
            _userInputCableThicknessLabel.color = _incorrectColour;
        }
        else
        {
            _userInputCableThicknessLabel.color = _correctColour;
        }
  
        if (_currentComparisonIndex == _faultPositionGuess.MaxCount() - 1)
        {
            if (scenarioSegment.length != 0)
            {
                _userInputCableLengthLabel.color = _correctColour;
            }
            else
            {
                _userInputCableLengthLabel.color = _incorrectColour;
            }
            return;
        }

        float lowerCorrectBound = scenarioSegment.length * (1 - CORRECT_BOUND);
        float upperCorrectBound = scenarioSegment.length * (1 + CORRECT_BOUND);

        float lowerMedianBound = scenarioSegment.length * (1 - MEDIAN_BOUND);
        float upperMedianBound = scenarioSegment.length * (1 + MEDIAN_BOUND);

        if (lowerCorrectBound < userSegment.length && userSegment.length < upperCorrectBound)
        {
            _userInputCableLengthLabel.color = _correctColour;
        }
        else if (lowerMedianBound < userSegment.length && userSegment.length < upperMedianBound)
        {
            _userInputCableLengthLabel.color = _medianColour;
        }
        else
        {
            _userInputCableLengthLabel.color = _incorrectColour;
        }
    }

    public void PreviousSegmentComparison()
    {
        if (_currentComparisonIndex == 0)
        {
            _currentComparisonIndex = _faultPositionGuess.MaxCount();
        }

        _currentComparisonIndex--;
        DisplayComparison();
    }

    public void NextSegmentComparison()
    {
        _currentComparisonIndex++;

        if (_currentComparisonIndex == _faultPositionGuess.MaxCount())
        {
            _currentComparisonIndex = 0;
        }

        DisplayComparison();
    }

    public void SetPopupActive(bool state)
    {
        gameObject.SetActive(state);
    }
}
