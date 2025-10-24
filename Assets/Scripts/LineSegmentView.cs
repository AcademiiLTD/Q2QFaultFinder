using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineSegmentView : MonoBehaviour
{
    [SerializeField] private Image _lineImage, _startPoint, _endPoint, _lengthLabel;
    [SerializeField] private TextMeshProUGUI _lengthLabelText;

    private float _mapMetersPerPixel;
    private float _lineScaledDistance;

    public void SetFirstPosition(Vector2 position, float metersPerPixel)
    {
        _mapMetersPerPixel = metersPerPixel;
        _startPoint.transform.position = position;
    }

    public void SetSecondPosition(Vector2 position)
    {
        _endPoint.enabled = true;
        _lineImage.enabled = true;

        Vector2 startPosition = _startPoint.transform.localPosition;
        _endPoint.transform.position = Input.mousePosition;
        _endPoint.transform.localPosition = new Vector3(
            Mathf.Clamp(_endPoint.transform.localPosition.x, -960f, 960f),
            Mathf.Clamp(_endPoint.transform.localPosition.y, -540f, 540f),
            0f);

        Vector2 endPosition = _endPoint.transform.localPosition;

        Vector2 dir = startPosition - endPosition;
        _lineImage.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        Vector3 midPoint = (startPosition + endPosition) / 2f;
        _lineImage.rectTransform.sizeDelta = new Vector2(Mathf.Abs(dir.magnitude), 20f);
        _lineImage.transform.localPosition = midPoint;

        _lineScaledDistance = _lineImage.rectTransform.sizeDelta.x / _mapMetersPerPixel;
        _lengthLabel.transform.localPosition = _lineImage.transform.localPosition + new Vector3(0f, 50f, 0f);
        Debug.Log(_lengthLabel.transform.position);
        _lengthLabel.transform.localPosition = new Vector3(
            Mathf.Clamp(_lengthLabel.transform.localPosition.x, -860f, 860f),
            Mathf.Clamp(_lengthLabel.transform.localPosition.y, -500f, 500f),
            0f);
    }

    public Vector3 StartPosition()
    {
        return _startPoint.transform.position;
    }

    public Vector3 EndPoisition()
    {
        return _endPoint.transform.position;
    }

    public void EndPointShowState(bool endPointShown)
    {
        _endPoint.enabled = endPointShown;
    }

    public float Length()
    {
        return _lineScaledDistance;
    }

    public void SetColour(Color colour)
    {
        _lineImage.color = colour;
    }

    public void SetLengthLabel(float sectionDisatance)
    {
        _lengthLabelText.text = $"{sectionDisatance.ToString("0.00")}m";
        _lengthLabel.gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_lengthLabel.rectTransform);
    }

    public void DisableLengthLabel()
    {
        _lengthLabel.gameObject.SetActive(false);
    }
}
