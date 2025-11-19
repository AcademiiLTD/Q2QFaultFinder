using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slideshow : MonoBehaviour
{
    [Header("Slides")]
    [SerializeField] private SlideCollection _slideCollection;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _bodyOnlyText;
    [SerializeField] private TextMeshProUGUI _mediaBodyText;

    [Header("Media")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Image _image;

    [Header("Delay")]
    [SerializeField] private float _buttonEnableDelaySeconds;

    private int _currentSlideIndex;
    private int _maxSlideIndex;

    [ContextMenu("Start")]
    private void OnEnable()
    {
        if (_slideCollection == null)
        {
            Debug.LogError($"Slide collection not found for {gameObject.name}");
            return;
        }

        _maxSlideIndex = _slideCollection.CollectionMaximumIndex();
        if (_maxSlideIndex == -1)
        {
            Debug.LogError($"Slide collection: {_slideCollection.CollectionName()} contains zero slides");
            return;
        }

        _currentSlideIndex = 0;
    }

    private void Start()
    {
        StartCoroutine(PopulateCurrentSlide());
    }

    [ContextMenu("Next slide")]
    public void NextSlide()
    {
        if (_currentSlideIndex >= _maxSlideIndex)
        {
            return;
        }

        _currentSlideIndex++;
        StartCoroutine(PopulateCurrentSlide());
    }

    [ContextMenu("Prev slide")]
    public void PreviousSlide()
    {
        if (_currentSlideIndex <= 0)
        {
            return;
        }

        _currentSlideIndex--;
        StartCoroutine(PopulateCurrentSlide());
    }

    private IEnumerator PopulateCurrentSlide()
    {
        ClearPopupFields();

        Slide currentSlide = _slideCollection.SlideFromIndex(_currentSlideIndex);
        currentSlide.DisplayUsingReferences(this);

        //Slight delay to prevent buttons from being spammed
        yield return new WaitForSeconds(_buttonEnableDelaySeconds);

        if (_maxSlideIndex == 0)
        {
            SlideshowDisplayEvents.InvokeSingleSlide();
        }
        else if (_currentSlideIndex == _maxSlideIndex)
        {
            SlideshowDisplayEvents.InvokeLastSlide();
        }
        else if (_currentSlideIndex == 0)
        {
            SlideshowDisplayEvents.InvokeFirstSlide();
        }
        else
        {
            SlideshowDisplayEvents.InvokeMiddlingSlide();
        }
    }

    [ContextMenu("Clear")]
    private void ClearPopupFields()
    {
        _titleText.text = string.Empty;
        _bodyOnlyText.text = string.Empty;
        _bodyOnlyText.gameObject.SetActive(false);

        _mediaBodyText.text = string.Empty;
        _mediaBodyText.gameObject.SetActive(false);

        _image.sprite = null;
        _image.gameObject.SetActive(false);

        _audioSource.Stop();

        SlideshowDisplayEvents.InvokeClearSlide();
    }

    public TextMeshProUGUI TitleText() => _titleText;
    public TextMeshProUGUI BodyOnlyText() => _bodyOnlyText;
    public TextMeshProUGUI MediaBodyText() => _mediaBodyText;
    public AudioSource AudioSource() => _audioSource;
    public Image Image() => _image;
}