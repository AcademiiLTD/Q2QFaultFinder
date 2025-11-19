using UnityEngine;

[System.Serializable]
public abstract class Slide
{
    [Header("Attributes")]
    [SerializeField] protected string _titleText;
    [TextArea(10, 20)]
    [SerializeField] protected string _bodyText;
    [SerializeField] protected AudioClip _text2SpeechClip;

    public abstract void DisplayUsingReferences(Slideshow popupSlideDisplay);
}
