using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourSelector : MonoBehaviour
{
    [SerializeField] private Color _myColour;
    [SerializeField] private Image _thumbnailImage;

    public void SetColour(Color colour)
    {
        _myColour = colour;
        _thumbnailImage.color = colour;
    }
}
