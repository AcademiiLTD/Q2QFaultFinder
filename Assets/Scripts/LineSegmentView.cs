using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineSegmentView : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Image _lineImage, _startPoint, _endPoint;

    private Vector2 _firstPosition, _secondPosition;

    public void SetFirstPosition(Vector2 position)
    {
        _firstPosition = position;
        _startPoint.enabled = true;
        _startPoint.transform.position = _firstPosition;

        //_lineRenderer.positionCount++;
        //_lineRenderer.SetPosition(0, position);
    }

    public void SetSecondPosition(Vector2 position)
    {
        _secondPosition = position;
        CreateLine();
        //_lineRenderer.positionCount++;
        //_lineRenderer.SetPosition(1, position);
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
}
