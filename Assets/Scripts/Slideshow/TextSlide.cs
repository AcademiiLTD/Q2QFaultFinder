using UnityEngine;

[System.Serializable]
public class TextSlide : Slide
{
    public override void DisplayUsingReferences(Slideshow popupSlideDisplay)
    {
        popupSlideDisplay.TitleText().text = _titleText;

        popupSlideDisplay.BodyOnlyText().text = _bodyText;
        popupSlideDisplay.BodyOnlyText().gameObject.SetActive(true);

        if (_text2SpeechClip == null)
        {
            Debug.LogWarning($"Audio clip not found for slide {_titleText}");
            return;
        }
        popupSlideDisplay.AudioSource().PlayOneShot(_text2SpeechClip);
    }
}
