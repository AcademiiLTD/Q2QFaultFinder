using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineSegmentView : MonoBehaviour
{
    [SerializeField] private Image _lineImage, _startPoint, _endPoint, _lengthLabel;
    [SerializeField] private TextMeshProUGUI _lengthLabelText;

    private Vector2 _firstPosition, _secondPosition;

    public void SetFirstPosition(Vector2 position)
    {
        _firstPosition = position;
        _startPoint.enabled = true;
        _startPoint.transform.position = _firstPosition;
    }

    public void SetSecondPosition(Vector2 position)
    {
        _secondPosition = position;
        CreateLine();
    }

    private void CreateLine()
    {

        Vector2 dir = _firstPosition - _secondPosition;
        _lineImage.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Vector3 midPoint = (_firstPosition + _secondPosition) / 2f;
        _lineImage.transform.localScale = new Vector3(dir.magnitude, 1f, 1f);
        _lineImage.transform.position = midPoint;

        _startPoint.enabled = true;
        _startPoint.transform.position = _firstPosition;
        _endPoint.enabled = true;
        _endPoint.transform.position = _secondPosition;

        _lineImage.enabled = true;
    }

    public float Length()
    {
        return _lineImage.transform.localScale.x;
    }

    public void SetColour(Color colour)
    {
        _lineImage.color = colour;
    }

    public void SetLength(string lengthValue)
    {
        _lengthLabelText.text = lengthValue;
        _lengthLabel.transform.position = _lineImage.transform.position + new Vector3(0f, 50f, 0f);
        _lengthLabel.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_lengthLabel.rectTransform);
    }
}
