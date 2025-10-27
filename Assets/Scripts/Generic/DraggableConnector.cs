using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class DraggableConnector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _myImage;
    [SerializeField] private GameObject _label;
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private Vector2 _overlapBoxSize;
    [SerializeField] private LayerMask _overlapLayerMask;
    [TextArea]
    public string _hintText;

    public static event Action<Transform, PointerEventData> OnGrabbedDraggable, OnReleasedDraggable;
    public ConnectorType _connectorType;

    private void Start()
    {
        _startPosition = transform.position;    
    }

    public void OnBeginDrag(PointerEventData data)
    {
        OnGrabbedDraggable(transform, data);
        ToggleLabel(true);
    }

    public void OnDrag(PointerEventData data)
    {
        transform.position = data.position;
    }

    public void OnEndDrag(PointerEventData data)
    {
        OnReleasedDraggable(transform, data);
        ToggleLabel(false);

        Collider2D[] foundColliders = Physics2D.OverlapBoxAll(transform.position, _overlapBoxSize, 0f, _overlapLayerMask);
        Collider2D closestCollider = null;

        foreach (Collider2D collider in foundColliders)
        {
            if (closestCollider == null)
            {
                closestCollider = collider;
                continue;
            }

            if (Vector2.Distance(transform.position, collider.transform.position) < Vector2.Distance(transform.position, closestCollider.transform.position))
            {
                closestCollider = collider;
            }
        }

        if (closestCollider != null)
        {
            closestCollider.GetComponent<ConnectorPoint>().TryPlaceConnector(this);
        }
        else
        {
            ResetConnectorPosition();
        }
    }

    public void ResetConnectorPosition()
    {
        transform.position = _startPosition;

    }

    public void ToggleConnectorActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void ToggleLabel(bool state)
    {
        _label.SetActive(state);
    }
}
