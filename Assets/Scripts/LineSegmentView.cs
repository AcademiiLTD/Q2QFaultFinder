using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineSegmentView : MonoBehaviour
{
    [SerializeField] private Image _lineImage, _startPoint, _endPoint, _lengthLabel;
    [SerializeField] private TextMeshProUGUI _lengthLabelText;
    private bool _placingLine;
    private float _mapMetersPerPixel;
    private float _lineScaledDistance;

    //private Vector2 _firstPosition, _secondPosition;

    public void SetFirstPosition(Vector2 position, float metersPerPixel)
    {
        _mapMetersPerPixel = metersPerPixel;
        _startPoint.enabled = true;
        _startPoint.transform.position = position;
        StartCoroutine(TrackMousePosition());
    }

    public void SetSecondPosition(Vector2 position)
    {
        //_secondPosition = position;
        //_endPoint.enabled = true;
        //_endPoint.transform.position = position;
        //CreateLine();
        _placingLine = false;
    }

    private void CreateLine()
    {
        Vector2 startPosition = _startPoint.transform.localPosition;
        Vector2 endPosition = _endPoint.transform.localPosition;

        Vector2 dir = startPosition - endPosition;
        _lineImage.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Vector3 midPoint = (startPosition + endPosition) / 2f;
        _lineImage.rectTransform.sizeDelta = new Vector2(Mathf.Abs(dir.magnitude), 20f);
        _lineImage.transform.localPosition = midPoint;
        _lineImage.enabled = true;

        _lineScaledDistance = _lineImage.rectTransform.sizeDelta.x / _mapMetersPerPixel;
        _lengthLabelText.text = $"{_lineScaledDistance.ToString("0.00")}m";
        _lengthLabel.transform.localPosition = _lineImage.transform.localPosition + new Vector3(0f, 50f, 0f);
        _lengthLabel.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_lengthLabel.rectTransform);

    }

    public float Length()
    {
        return _lineScaledDistance;
    }

    public void SetColour(Color colour)
    {
        _lineImage.color = colour;
    }

    private IEnumerator TrackMousePosition()
    {
        _placingLine = true;
        _endPoint.enabled = true;

        while (_placingLine)
        {
            Vector2 startPosition = _startPoint.transform.localPosition;
            _endPoint.transform.position = Input.mousePosition;
            Vector2 endPosition = _endPoint.transform.localPosition;

            Vector2 dir = startPosition - endPosition;
            _lineImage.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            Vector3 midPoint = (startPosition + endPosition) / 2f;
            _lineImage.rectTransform.sizeDelta = new Vector2(Mathf.Abs(dir.magnitude), 20f);
            _lineImage.transform.localPosition = midPoint;
            _lineImage.enabled = true;

            _lineScaledDistance = _lineImage.rectTransform.sizeDelta.x / _mapMetersPerPixel;
            _lengthLabelText.text = $"{_lineScaledDistance.ToString("0.00")}m";
            _lengthLabel.transform.localPosition = _lineImage.transform.localPosition + new Vector3(0f, 50f, 0f);
            _lengthLabel.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_lengthLabel.rectTransform);
            yield return null;
        }
    }
}
