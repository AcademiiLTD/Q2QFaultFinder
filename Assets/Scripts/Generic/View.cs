using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _viewCanvasGroup;

    public void ToggleView(bool state)
    {
        _viewCanvasGroup.alpha = state ? 1f : 0f;
        _viewCanvasGroup.interactable = state;
        _viewCanvasGroup.blocksRaycasts = state;
    }
}
