using UnityEngine;

[System.Serializable]
public class ImageSlide : Slide
{
    [Header("Varient Attributes")]
    [SerializeField] private Sprite _image;

    public override void DisplayUsingReferences(Slideshow popupSlideDisplay)
    {
        popupSlideDisplay.TitleText().text = _titleText;

        popupSlideDisplay.MediaBodyText().text = _bodyText;
        popupSlideDisplay.MediaBodyText().gameObject.SetActive(true);

        popupSlideDisplay.Image().sprite = _image;
        popupSlideDisplay.Image().gameObject.SetActive(true);

        if (_text2SpeechClip == null)
        {
            Debug.LogWarning($"Audio clip not found for slide {_titleText}");
            return;
        }
        popupSlideDisplay.AudioSource().PlayOneShot(_text2SpeechClip);
    }
}
