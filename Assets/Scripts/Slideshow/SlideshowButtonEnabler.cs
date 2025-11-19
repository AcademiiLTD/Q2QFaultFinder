using UnityEngine;

public class SlideshowButtonEnabler : MonoBehaviour
{
    [Header("Next Button")]
    [SerializeField] private GameObject nextButton;

    [Header("Previous Button")]
    [SerializeField] private GameObject previousButton;

    [Header("Continue Button")]
    [SerializeField] private GameObject continueButton;

    private void OnEnable()
    {
        SlideshowDisplayEvents.FirstSlide += FirstSlide;
        SlideshowDisplayEvents.LastSlide += LastSlide;
        SlideshowDisplayEvents.MiddlingSlide += MiddlingSlide;
        SlideshowDisplayEvents.SingleSlide += SingleSlide;
        SlideshowDisplayEvents.ClearSlide += ClearSlide;
    }

    private void OnDisable()
    {
        SlideshowDisplayEvents.FirstSlide -= FirstSlide;
        SlideshowDisplayEvents.LastSlide -= LastSlide;
        SlideshowDisplayEvents.MiddlingSlide -= MiddlingSlide;
        SlideshowDisplayEvents.SingleSlide += SingleSlide;
        SlideshowDisplayEvents.ClearSlide -= ClearSlide;
    }

    private void FirstSlide()
    {
        nextButton.SetActive(true);
    }

    private void LastSlide()
    {
        previousButton.SetActive(true);
        continueButton.SetActive(true);
    }

    private void MiddlingSlide()
    {
        nextButton.SetActive(true);
        previousButton.SetActive(true);
    }

    private void SingleSlide()
    {
        continueButton.SetActive(true);
    }

    private void ClearSlide()
    {
        nextButton.SetActive(false);
        previousButton.SetActive(false);
        continueButton.SetActive(false);
    }
}
