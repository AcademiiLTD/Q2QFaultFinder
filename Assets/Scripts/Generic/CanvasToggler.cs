using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasToggler : MonoBehaviour
{
    [SerializeField] private bool _startsVisible;
    [SerializeField] protected CanvasGroup _viewCanvasGroup;
    [SerializeField] protected Animator _viewAnimator;

    private void OnValidate()
    {
        if (!_viewCanvasGroup)
        {
            if (TryGetComponent<CanvasGroup>(out CanvasGroup canvasGroup))
            {
                _viewCanvasGroup = canvasGroup;
            }
        }

        if (!_viewAnimator)
        {
            if (TryGetComponent<Animator>(out Animator animator))
            {
                _viewAnimator = animator;
            }
        }
    }

    private void Awake()
    {
        if (_viewAnimator) _viewAnimator.SetBool("Visible", _startsVisible);
    }

    public void ToggleView(bool state)
    {
        if (_viewAnimator) _viewAnimator.SetBool("Visible", state);
    }
}
