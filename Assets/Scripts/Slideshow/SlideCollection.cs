using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PopupSlideCollection")]
public class SlideCollection : ScriptableObject
{
    [Header("Attribute")]
    [SerializeField] private string _slideCollectionName;

    [Header("Slide Collection")]
    [SerializeField] [SerializeReference] private List<Slide> _slides = new List<Slide>();

    public Slide SlideFromIndex(int index)
    {
        if (_slides.Count < index || index < 0)
        {
            Debug.LogError($"Slide index {index} not found in {_slideCollectionName}");
            return null;
        }

        return _slides[index];
    }

    public int CollectionMaximumIndex() => _slides.Count - 1;
    public string CollectionName() => _slideCollectionName;

    public void AddTextSlide()
    {
        _slides.Add(new TextSlide());
    }

    public void AddImageSlide()
    {
        _slides.Add(new ImageSlide());
    }
}
