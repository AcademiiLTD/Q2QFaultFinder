using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _viewCanvasGroup;
    [SerializeField] protected Animator _viewAnimator;
    [SerializeField] private bool _fadeInView;

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

    public void ToggleView(bool state)
    {
        if (_fadeInView)
        {
            //use animator
        }
        else
        {
            _viewCanvasGroup.alpha = state ? 1f : 0f;
            _viewCanvasGroup.interactable = state;
            _viewCanvasGroup.blocksRaycasts = state;
        }

    }
}
